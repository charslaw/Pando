using System;
using Pando.DataSources;
using Pando.Exceptions;
using Pando.Serialization;

namespace Pando.Repositories;

public class PandoRepository<T>(INodeDataStore nodeDataStore, ISnapshotDataStore snapshotDataStore, IPandoSerializer<T> serializer)
	: IPandoRepository<T>
{
	public PandoRepository(IPandoSerializer<T> serializer) : this(new MemoryNodeStore(), new MemorySnapshotStore(), serializer) { }

	public SnapshotId SaveRootSnapshot(T tree)
	{
		if (snapshotDataStore.RootSnapshot is not null) throw new AlreadyHasRootSnapshotException();

		var rootNodeId = SerializeToNodeId(tree);
		return snapshotDataStore.AddRootSnapshot(rootNodeId);
	}

	public SnapshotId SaveSnapshot(T tree, SnapshotId parentSnapshotId)
	{
		if (!snapshotDataStore.HasSnapshot(parentSnapshotId))
		{
			throw new SnapshotIdNotFoundException(parentSnapshotId, nameof(parentSnapshotId));
		}

		var rootNodeId = SerializeToNodeId(tree);
		return snapshotDataStore.AddSnapshot(rootNodeId, parentSnapshotId);
	}

	/// Merges the two snapshots identified by the given hashes, returning the hash of the merged result snapshot.
	/// Conflict resolution is determined by the passed in <see cref="IPandoSerializer{T}"/>.
	public SnapshotId MergeSnapshots(SnapshotId sourceSnapshotId, SnapshotId targetSnapshotId)
	{
		if (sourceSnapshotId == targetSnapshotId) throw new InvalidMergeException($"cannot merge a snapshot with itself ({sourceSnapshotId})");

		var baseSnapshotHash = snapshotDataStore.GetSnapshotLeastCommonAncestor(sourceSnapshotId, targetSnapshotId);

		if (baseSnapshotHash == targetSnapshotId) throw new InvalidMergeException("cannot merge a snapshot into one of its ancestors");
		if (baseSnapshotHash == sourceSnapshotId) throw new InvalidMergeException("cannot merge a snapshot into one of its descendants");

		Span<byte> idBuffer = stackalloc byte[NodeId.SIZE * 3];
		var baseNodeIdBuffer = idBuffer.Slice(0, NodeId.SIZE);
		var targetNodeIdBuffer = idBuffer.Slice(NodeId.SIZE, NodeId.SIZE);
		var sourceNodeIdBuffer = idBuffer.Slice(NodeId.SIZE * 2, NodeId.SIZE);
		snapshotDataStore.GetSnapshotData(baseSnapshotHash).RootNodeId.CopyTo(baseNodeIdBuffer);
		snapshotDataStore.GetSnapshotData(targetSnapshotId).RootNodeId.CopyTo(targetNodeIdBuffer);
		snapshotDataStore.GetSnapshotData(sourceSnapshotId).RootNodeId.CopyTo(sourceNodeIdBuffer);

		serializer.Merge(
			baseNodeIdBuffer,
			targetNodeIdBuffer,
			sourceNodeIdBuffer,
			nodeDataStore
		);

		var mergedNodeId = NodeId.FromBuffer(baseNodeIdBuffer);

		var mergeSnapshotId = snapshotDataStore.AddSnapshot(mergedNodeId, sourceSnapshotId, targetSnapshotId);
		return mergeSnapshotId;
	}

	public void WalkSnapshots(SnapshotVisitor<T> visitor) =>
		snapshotDataStore.WalkTree((snapshotId, sourceSnapshotId, targetSnapshotId, nodeId) =>
			visitor(snapshotId, DeserializeFromNodeId(nodeId), sourceSnapshotId, targetSnapshotId)
		);


	public T GetSnapshot(SnapshotId snapshotId)
	{
		var nodeId = snapshotDataStore.GetSnapshotData(snapshotId).RootNodeId;
		return DeserializeFromNodeId(nodeId);
	}

	private NodeId SerializeToNodeId(T tree)
	{
		Span<byte> idBuffer = stackalloc byte[NodeId.SIZE];
		serializer.Serialize(tree, idBuffer, nodeDataStore);
		return NodeId.FromBuffer(idBuffer);
	}

	private T DeserializeFromNodeId(NodeId nodeId)
	{
		Span<byte> idBuffer = stackalloc byte[sizeof(ulong)];
		nodeId.CopyTo(idBuffer);
		return serializer.Deserialize(idBuffer, nodeDataStore);
	}
}
