using System.Collections.Generic;
using Pando.Exceptions;
using Pando.Repositories;

namespace Pando.DataSources;

public interface IReadOnlySnapshotDataStore
{
	/// The total number of snapshots in this data source.
	int SnapshotCount { get; }

	/// Returns the root snapshot of the tree.
	SnapshotId? RootSnapshot { get; }

	/// Returns whether a snapshot identified by the given <see cref="SnapshotId"/> exists in the data source.
	bool HasSnapshot(SnapshotId snapshotId);

	/// Returns data about the snapshot with the given snapshot id.
	/// <exception cref="SnapshotIdNotFoundException">thrown if there is no snapshot node identified by the given <see cref="SnapshotId"/>.</exception>
	SnapshotData GetSnapshotData(SnapshotId snapshotId);

	/// Enumerates the child ids of the snapshot with the given id.
	IEnumerable<SnapshotId> GetSnapshotChildren(SnapshotId snapshotId);

	/// Returns the <see cref="SnapshotId"/> of the least common ancestor snapshot of the two snapshots identified by the given <see cref="SnapshotId"/>s.
	SnapshotId GetSnapshotLeastCommonAncestor(SnapshotId id1, SnapshotId id2);

	/// Walks through the tree, depth first, in order of original insertion into the tree.
	void WalkTree(TreeEntryVisitor visitor);
}

public delegate void TreeEntryVisitor(
	SnapshotId snapshotId,
	SnapshotId sourceSnapshotId,
	SnapshotId targetSnapshotId,
	NodeId rootNodeId
);

public interface ISnapshotDataStore : IReadOnlySnapshotDataStore
{
	/// Adds a root snapshot to the tree.
	/// <exception cref="AlreadyHasRootSnapshotException">if a root snapshot already exists in the tree.</exception>
	public SnapshotId AddRootSnapshot(NodeId rootNodeId);

	/// Adds a snapshot to the tree with the given source parent (non merged).
	public SnapshotId AddSnapshot(
		NodeId rootNodeId,
		SnapshotId sourceSnapshotId,
		SnapshotId targetSnapshotId = default
	);
}
