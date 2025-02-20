using System;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.DataStructures;
using Pando.Exceptions;
using Pando.Serialization;

namespace Pando.Repositories;

public class PandoRepository<T> : IRepository<T>
{
	private readonly IDataSource _dataSource;
	private readonly IPandoSerializer<T> _serializer;

	public SnapshotTree SnapshotTree { get; }

	public PandoRepository(IDataSource dataSource, IPandoSerializer<T> serializer)
	{
		_dataSource = dataSource;
		_serializer = serializer;
		SnapshotTree = new SnapshotTree();
	}

	public SnapshotId SaveRootSnapshot(T tree)
	{
		if (_dataSource.SnapshotCount > 0) throw new AlreadyHasRootSnapshotException();

		var nodeHash = SerializeToNodeId(tree);
		var snapshotHash = _dataSource.AddSnapshot(SnapshotId.None, nodeHash);
		return snapshotHash;
	}

	public SnapshotId SaveSnapshot(T tree, SnapshotId parentSnapshotId)
	{
		if (!_dataSource.HasSnapshot(parentSnapshotId)) throw new SnapshotIdNotFoundException(parentSnapshotId, nameof(parentSnapshotId));

		var nodeHash = SerializeToNodeId(tree);
		var snapshotHash = _dataSource.AddSnapshot(parentSnapshotId, nodeHash);
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
		return mergeSnapshotId;
	}

	public T GetSnapshot(SnapshotId snapshotId)
	{
		var nodeHash = _dataSource.GetSnapshotRootNode(snapshotId);
		return DeserializeFromNodeId(nodeHash);
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
