using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Pando.DataSources.Utils;
using Pando.Exceptions;

namespace Pando.DataSources;

public class MemoryDataSource : IDataSource
{
	private readonly Dictionary<SnapshotId, SnapshotData> _snapshotIndex;
	private readonly Dictionary<NodeId, Range> _nodeIndex;
	private readonly SpannableList<byte> _nodeData;

	public MemoryDataSource()
	{
		_snapshotIndex = new Dictionary<SnapshotId, SnapshotData>();
		_nodeIndex = new Dictionary<NodeId, Range>();
		_nodeData = new SpannableList<byte>();
	}

	public MemoryDataSource(Stream snapshotIndexSource, Stream nodeIndexSource, Stream nodeDataSource)
		: this(
			snapshotIndexSource: snapshotIndexSource,
			nodeIndexSource: nodeIndexSource,
			nodeDataSource: nodeDataSource,
			null, null, null
		) { }

	internal MemoryDataSource(
		Dictionary<SnapshotId, SnapshotData>? snapshotIndex = null,
		Dictionary<NodeId, Range>? nodeIndex = null,
		SpannableList<byte>? nodeData = null
	)
	{
		_snapshotIndex = snapshotIndex ?? new Dictionary<SnapshotId, SnapshotData>();
		_nodeIndex = nodeIndex ?? new Dictionary<NodeId, Range>();
		_nodeData = nodeData ?? new SpannableList<byte>();
	}


	internal MemoryDataSource(
		Stream snapshotIndexSource, Stream nodeIndexSource, Stream nodeDataSource,
		Dictionary<SnapshotId, SnapshotData>? snapshotIndex = null,
		Dictionary<NodeId, Range>? nodeIndex = null,
		SpannableList<byte>? nodeData = null
	)
	{
		_snapshotIndex = StreamUtils.SnapshotIndex.PopulateSnapshotIndex(snapshotIndexSource, snapshotIndex);
		_nodeIndex = StreamUtils.NodeIndex.PopulateNodeIndex(nodeIndexSource, nodeIndex);
		_nodeData = StreamUtils.NodeData.PopulateNodeData(nodeDataSource, nodeData);
	}


#region INodeDataSink Implementation

	/// <remarks>This implementation is guaranteed not to insert duplicate nodes.</remarks>
	/// <inheritdoc/>
	public NodeId AddNode(ReadOnlySpan<byte> bytes)
	{
		var nodeId = HashUtils.ComputeNodeHash(bytes);
		if (HasNode(nodeId)) return nodeId;

		AddNodeWithIdUnsafe(nodeId, bytes);
		return nodeId;
	}

	/// Adds a new node indexed with the given id, containing the given bytes.
	/// <remarks>
	///     <para>This method is unsafe because the given id and data might mismatch,
	///     and because it will blindly add the data to the node data collection
	///     even if it already exists in the collection.</para>
	///     <para>When calling this method, ensure the correct id is given and
	///     call <see cref="HasNode"/> first to ensure that this is not a duplicate node.</para>
	/// </remarks>
	internal void AddNodeWithIdUnsafe(NodeId nodeId, ReadOnlySpan<byte> bytes)
	{
		var dataSlice = _nodeData.AddSpan(bytes);
		_nodeIndex.Add(nodeId, dataSlice);
	}

#endregion

#region ISnapshotDataSink Implementation

	public SnapshotId AddSnapshot(SnapshotId parentSnapshotId, NodeId rootNodeId)
	{
		var snapshotId = HashUtils.ComputeSnapshotHash(parentSnapshotId, rootNodeId);
		if (HasSnapshot(snapshotId)) return snapshotId;

		AddSnapshotWithIdUnsafe(snapshotId, parentSnapshotId, rootNodeId);
		return snapshotId;
	}

	/// <remarks>This method is unsafe because the given id and data might mismatch.
	/// When calling this method, ensure the correct id is given.</remarks>
	internal void AddSnapshotWithIdUnsafe(SnapshotId snapshotId, SnapshotId parentSnapshotId, NodeId rootNodeId)
	{
		SnapshotData snapshotData = new SnapshotData(parentSnapshotId, rootNodeId);
		_snapshotIndex.Add(snapshotId, snapshotData);
	}

#endregion

#region INodeDataSource Implementation

	public bool HasNode(NodeId nodeId) => _nodeIndex.ContainsKey(nodeId);

	public int GetSizeOfNode(NodeId nodeId)
	{
		EnsureNodePresence(nodeId, nameof(nodeId));
		var (_, dataLength) = _nodeIndex[nodeId].GetOffsetAndLength(_nodeData.Count);
		return dataLength;
	}

	public void CopyNodeBytesTo(NodeId nodeId, Span<byte> outputBytes)
	{
		EnsureNodePresence(nodeId, nameof(nodeId));
		_nodeData.CopyTo(_nodeIndex[nodeId], outputBytes);
	}

#endregion

#region ISnapshotDataSource Implementation

	public int SnapshotCount => _snapshotIndex.Count;

	public bool HasSnapshot(SnapshotId snapshotId) => _snapshotIndex.ContainsKey(snapshotId);

	public SnapshotId GetSnapshotParent(SnapshotId snapshotId)
	{
		EnsureSnapshotPresence(snapshotId, nameof(snapshotId));
		return _snapshotIndex[snapshotId].ParentSnapshotId;
	}

	public SnapshotId GetSnapshotLeastCommonAncestor(SnapshotId id1, SnapshotId id2)
	{
		EnsureSnapshotPresence(id1, nameof(id1));
		EnsureSnapshotPresence(id2, nameof(id2));

		HashSet<SnapshotId> snapshot1Ancestors = [];
		var current = id1;
		while (current != SnapshotId.None)
		{
			snapshot1Ancestors.Add(current);
			current = _snapshotIndex[current].ParentSnapshotId;
		}

		current = id2;
		while (current != SnapshotId.None)
		{
			if (snapshot1Ancestors.Contains(current)) return current;
			current = _snapshotIndex[current].ParentSnapshotId;
		}

		throw new Exception("Given snapshots don't have a common ancestor");
	}

	public NodeId GetSnapshotRootNode(SnapshotId snapshotId)
	{
		EnsureSnapshotPresence(snapshotId, nameof(snapshotId));
		return _snapshotIndex[snapshotId].RootNodeId;
	}

#endregion

	private void EnsureNodePresence(NodeId nodeId, string paramName)
	{
		if (!HasNode(nodeId)) throw new NodeIdNotFoundException(nodeId, paramName);
	}

	private void EnsureSnapshotPresence(SnapshotId snapshotId, string paramName)
	{
		if (!HasSnapshot(snapshotId)) throw new SnapshotIdNotFoundException(snapshotId, paramName);
	}
}
