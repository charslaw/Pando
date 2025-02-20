using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.Immutable;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Exceptions;
using Pando.Repositories.Utils;
using Pando.Serialization;

namespace Pando.Repositories;

public class PandoRepository<T> : IRepository<T>
{
	private readonly IDataSource _dataSource;
	private readonly IPandoSerializer<T> _serializer;

	private SnapshotId? _rootSnapshot;
	private readonly Dictionary<SnapshotId, SmallSet<SnapshotId>> _snapshotTreeElements = new();

	public PandoRepository(IDataSource dataSource, IPandoSerializer<T> serializer)
	{
		_dataSource = dataSource;
		_serializer = serializer;

		var snapshotCount = _dataSource.SnapshotCount;
		if (snapshotCount > 0)
		{
			_snapshotTreeElements = new Dictionary<SnapshotId, SmallSet<SnapshotId>>(snapshotCount);
			InitializeSnapshotTree(_dataSource.GetLeafSnapshotIds());
		}
	}

	public SnapshotId SaveRootSnapshot(T tree)
	{
		if (_dataSource.SnapshotCount > 0) throw new AlreadyHasRootSnapshotException();

		var nodeHash = SerializeToNodeId(tree);
		var snapshotHash = _dataSource.AddSnapshot(SnapshotId.None, nodeHash);
		AddToSnapshotTree(snapshotHash);
		return snapshotHash;
	}

	public SnapshotId SaveSnapshot(T tree, SnapshotId parentSnapshotId)
	{
		if (!_dataSource.HasSnapshot(parentSnapshotId))
		{
			throw new HashNotFoundException(
				$"Could not save a snapshot with parent snapshot id {parentSnapshotId} because no snapshot with that id exists in the data source."
			);
		}

		var nodeHash = SerializeToNodeId(tree);
		var snapshotHash = _dataSource.AddSnapshot(parentSnapshotId, nodeHash);
		AddToSnapshotTree(snapshotHash, parentSnapshotId);
		return snapshotHash;
	}

	/// Merges the two snapshots identified by the given hashes, returning the hash of the merged result snapshot.
	/// Conflict resolution is determined by the passed in <see cref="IPandoSerializer{T}"/>.
	public SnapshotId MergeSnapshots(SnapshotId targetSnapshotId, SnapshotId sourceSnapshotId)
	{
		var baseSnapshotHash = _dataSource.GetSnapshotLeastCommonAncestor(targetSnapshotId, sourceSnapshotId);
		Span<byte> idBuffer = stackalloc byte[NodeId.SIZE * 3];
		var baseNodeIdBuffer = idBuffer.Slice(0, NodeId.SIZE);
		var targetNodeIdBuffer = idBuffer.Slice(NodeId.SIZE, NodeId.SIZE);
		var sourceNodeIdBuffer = idBuffer.Slice(NodeId.SIZE * 2, NodeId.SIZE);
		_dataSource.GetSnapshotRootNode(baseSnapshotHash).CopyTo(baseNodeIdBuffer);
		_dataSource.GetSnapshotRootNode(targetSnapshotId).CopyTo(targetNodeIdBuffer);
		_dataSource.GetSnapshotRootNode(sourceSnapshotId).CopyTo(sourceNodeIdBuffer);

		_serializer.Merge(
			baseNodeIdBuffer,
			targetNodeIdBuffer,
			sourceNodeIdBuffer,
			_dataSource
		);

		var mergedNodeId = NodeId.FromBuffer(baseNodeIdBuffer);

		var mergeSnapshotId = _dataSource.AddSnapshot(sourceSnapshotId, mergedNodeId);
		AddToSnapshotTree(mergeSnapshotId, sourceSnapshotId);
		return mergeSnapshotId;
	}

	public T GetSnapshot(SnapshotId snapshotId)
	{
		var nodeHash = _dataSource.GetSnapshotRootNode(snapshotId);
		return DeserializeFromNodeId(nodeHash);
	}

	public SnapshotTree GetSnapshotTree()
	{
		if (_rootSnapshot is null) throw new NoRootSnapshotException();

		return GetSnapshotTreeInternal(_rootSnapshot.Value);
	}

	private SnapshotTree GetSnapshotTreeInternal(SnapshotId snapshotId)
	{
		if (!_snapshotTreeElements.ContainsKey(snapshotId))
		{
			// This should not happen except in the case of developer error.
			// All calls to this method (except for the root snapshot) use a hash that should definitely be in the _snapshotTreeElements
			throw new HashNotFoundException(
				$"Could not get snapshot tree with root hash {snapshotId} because the given hash does not exist in the snapshot tree."
			);
		}

		var children = _snapshotTreeElements[snapshotId];
		var childrenCount = children.Count;
		switch (childrenCount)
		{
			case 0: return new SnapshotTree(snapshotId);
			case 1:
				var list = ImmutableArray.Create(GetSnapshotTreeInternal(children.Single));
				return new SnapshotTree(snapshotId, list);
			default:
				var treeChildren = ImmutableArray.CreateBuilder<SnapshotTree>(childrenCount);
				foreach (var childId in children.All)
				{
					treeChildren.Add(GetSnapshotTreeInternal(childId));
				}

				return new SnapshotTree(snapshotId, treeChildren.MoveToImmutable());
		}
	}

	private void InitializeSnapshotTree(IReadOnlySet<SnapshotId> leafIds)
	{
		foreach (var leafId in leafIds)
		{
			_snapshotTreeElements[leafId] = new SmallSet<SnapshotId>();

			var currentId = leafId;
			var parentId = _dataSource.GetSnapshotParent(currentId);
			while (parentId != SnapshotId.None)
			{
				if (_snapshotTreeElements.TryGetValue(parentId, out var set))
				{
					set.Add(currentId);
					_snapshotTreeElements[parentId] = set;
					break; // We've run into an existing hash; that means we've already explored everything above this.
				}

				_snapshotTreeElements.Add(parentId, new SmallSet<SnapshotId>(currentId));

				currentId = parentId;
				parentId = _dataSource.GetSnapshotParent(currentId);
			}

			if (parentId == SnapshotId.None) _rootSnapshot = currentId;
		}
	}

	private void AddToSnapshotTree(SnapshotId snapshotId)
	{
		_rootSnapshot = snapshotId;
		_snapshotTreeElements[snapshotId] = default;
	}

	private void AddToSnapshotTree(SnapshotId snapshotId, SnapshotId parentSnapshotId)
	{
		if (!_snapshotTreeElements.ContainsKey(parentSnapshotId))
		{
			// This should not happen except in the case of developer error
			// This method is only called after confirming that the parent hash already exists in the data source in `SaveSnapshot`
			throw new HashNotFoundException(
				$"Could not add the snapshot to the snapshot tree because the given parent snapshot id {parentSnapshotId} could not be found in the snapshot tree."
			);
		}

		_snapshotTreeElements[snapshotId] = default;
		var children = _snapshotTreeElements[parentSnapshotId];
		children.Add(snapshotId);
		_snapshotTreeElements[parentSnapshotId] = children;
	}

	private NodeId SerializeToNodeId(T tree)
	{
		Span<byte> idBuffer = stackalloc byte[NodeId.SIZE];
		_serializer.Serialize(tree, idBuffer, _dataSource);
		return NodeId.FromBuffer(idBuffer);
	}

	private T DeserializeFromNodeId(NodeId nodeId)
	{
		Span<byte> idBuffer = stackalloc byte[sizeof(ulong)];
		nodeId.CopyTo(idBuffer);
		return _serializer.Deserialize(idBuffer, _dataSource);
	}
}
