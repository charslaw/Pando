using System;
using System.Collections.Generic;
using Pando.DataSources.Utils;
using Pando.Exceptions;
using Pando.Repositories;

namespace Pando.DataSources;

public class MemoryNodeStore : INodeDataStore
{
	private readonly Dictionary<NodeId, Range> _nodeIndex;
	private readonly SpannableList<byte> _nodeData;

	public MemoryNodeStore()
	{
		_nodeIndex = new Dictionary<NodeId, Range>();
		_nodeData = new SpannableList<byte>();
	}

	internal MemoryNodeStore(
		Dictionary<NodeId, Range>? nodeIndex = null,
		SpannableList<byte>? nodeData = null
	)
	{
		_nodeIndex = nodeIndex ?? new Dictionary<NodeId, Range>();
		_nodeData = nodeData ?? new SpannableList<byte>();
	}

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

	private void EnsureNodePresence(NodeId nodeId, string paramName)
	{
		if (!HasNode(nodeId)) throw new NodeIdNotFoundException(nodeId, paramName);
	}
}
