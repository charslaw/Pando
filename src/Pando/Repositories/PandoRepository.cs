using System;
using Pando.Exceptions;
using Pando.Serializers;
using Pando.Vaults;

namespace Pando.Repositories;

public class PandoRepository<T>(INodeVault nodeVault, ISnapshotVault snapshotVault, IPandoSerializer<T> serializer)
	: IPandoRepository<T>
{
	public INodeVault NodeVault { get; } = nodeVault;
	public ISnapshotVault SnapshotVault { get; } = snapshotVault;

	public PandoRepository(IPandoSerializer<T> serializer)
		: this(new MemoryNodeVault(), new MemorySnapshotVault(), serializer) { }

	public SnapshotId SaveRootSnapshot(T tree)
	{
		if (SnapshotVault.RootSnapshot is not null)
			throw new AlreadyHasRootSnapshotException();

		var rootNodeId = SerializeToNodeId(tree);
		return SnapshotVault.AddRootSnapshot(rootNodeId);
	}

	public SnapshotId SaveSnapshot(T tree, SnapshotId parentSnapshotId)
	{
		if (!SnapshotVault.HasSnapshot(parentSnapshotId))
		{
			throw new SnapshotIdNotFoundException(parentSnapshotId, nameof(parentSnapshotId));
		}

		var rootNodeId = SerializeToNodeId(tree);
		return SnapshotVault.AddSnapshot(rootNodeId, parentSnapshotId);
	}

	/// Merges the two snapshots identified by the given hashes, returning the hash of the merged result snapshot.
	/// Conflict resolution is determined by the passed in <see cref="IPandoSerializer{T}"/>.
	public SnapshotId MergeSnapshots(SnapshotId sourceSnapshotId, SnapshotId targetSnapshotId)
	{
		if (sourceSnapshotId == targetSnapshotId)
			throw new InvalidMergeException($"cannot merge a snapshot with itself ({sourceSnapshotId})");

		var baseSnapshotHash = SnapshotVault.GetSnapshotLeastCommonAncestor(sourceSnapshotId, targetSnapshotId);

		if (baseSnapshotHash == targetSnapshotId)
			throw new InvalidMergeException("cannot merge a snapshot into one of its ancestors");
		if (baseSnapshotHash == sourceSnapshotId)
			throw new InvalidMergeException("cannot merge a snapshot into one of its descendants");

		Span<byte> idBuffer = stackalloc byte[NodeId.SIZE * 3];
		var baseNodeIdBuffer = idBuffer.Slice(0, NodeId.SIZE);
		var targetNodeIdBuffer = idBuffer.Slice(NodeId.SIZE, NodeId.SIZE);
		var sourceNodeIdBuffer = idBuffer.Slice(NodeId.SIZE * 2, NodeId.SIZE);
		SnapshotVault.GetSnapshotData(baseSnapshotHash).RootNodeId.CopyTo(baseNodeIdBuffer);
		SnapshotVault.GetSnapshotData(targetSnapshotId).RootNodeId.CopyTo(targetNodeIdBuffer);
		SnapshotVault.GetSnapshotData(sourceSnapshotId).RootNodeId.CopyTo(sourceNodeIdBuffer);

		serializer.Merge(baseNodeIdBuffer, targetNodeIdBuffer, sourceNodeIdBuffer, NodeVault);

		var mergedNodeId = NodeId.FromBuffer(baseNodeIdBuffer);

		var mergeSnapshotId = SnapshotVault.AddSnapshot(mergedNodeId, sourceSnapshotId, targetSnapshotId);
		return mergeSnapshotId;
	}

	public void WalkSnapshots(SnapshotVisitor<T> visitor) =>
		SnapshotVault.WalkTree(
			(snapshotId, sourceSnapshotId, targetSnapshotId, nodeId) =>
				visitor(snapshotId, DeserializeFromNodeId(nodeId), sourceSnapshotId, targetSnapshotId)
		);

	public T GetSnapshot(SnapshotId snapshotId)
	{
		var nodeId = SnapshotVault.GetSnapshotData(snapshotId).RootNodeId;
		return DeserializeFromNodeId(nodeId);
	}

	private NodeId SerializeToNodeId(T tree)
	{
		Span<byte> idBuffer = stackalloc byte[NodeId.SIZE];
		serializer.Serialize(tree, idBuffer, NodeVault);
		return NodeId.FromBuffer(idBuffer);
	}

	private T DeserializeFromNodeId(NodeId nodeId)
	{
		Span<byte> idBuffer = stackalloc byte[NodeId.SIZE];
		nodeId.CopyTo(idBuffer);
		return serializer.Deserialize(idBuffer, NodeVault);
	}
}
