using Pando.Exceptions;

namespace Pando.Repositories;

public interface IPandoRepository<T>
{
	/// Saves the given state tree snapshot as a root snapshot, then returns a <see cref="SnapshotId"/> of the snapshot that can be used to retrieve it.
	/// <exception cref="AlreadyHasRootSnapshotException">Thrown if this repository already has a root snapshot.
	/// Each repository can only have one root snapshot.</exception>
	SnapshotId SaveRootSnapshot(T tree);

	/// Saves the given state tree with the given parent snapshot, then returns a <see cref="SnapshotId"/> of the snapshot that can be used to retrieve it.
	/// <exception cref="SnapshotIdNotFoundException">Thrown if the given <paramref name="parentSnapshotId"/> does not exist.</exception>
	SnapshotId SaveSnapshot(T tree, SnapshotId parentSnapshotId);

	/// Retrieves the state tree identified by the given <see cref="SnapshotId"/>.
	/// <exception cref="SnapshotIdNotFoundException">Thrown if the given <paramref name="snapshotId"/> does not exist.</exception>
	T GetSnapshot(SnapshotId snapshotId);

	/// Merges the two snapshots identified by the given source and target snapshots, returning the <see cref="SnapshotId"/> of the merged result.
	/// Conflict resolution is implementation specific.
	/// <exception cref="SnapshotIdNotFoundException">Thrown if either of the given <see cref="SnapshotId"/>s do not exist.</exception>
	SnapshotId MergeSnapshots(SnapshotId sourceSnapshotId, SnapshotId targetSnapshotId);

	/// Walks through snapshots in the repository, starting from the root, depth-first.
	void WalkSnapshots(SnapshotVisitor<T> visitor);
}

public delegate void SnapshotVisitor<in T>(
	SnapshotId snapshotId,
	T node,
	SnapshotId sourceParentId,
	SnapshotId targetParentId
);
