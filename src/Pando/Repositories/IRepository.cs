using System.Collections.Immutable;
using Pando.Exceptions;

namespace Pando.Repositories;

public interface IRepository<T>
{
	/// Saves the given state tree snapshot as a root snapshot, then returns a hash of the snapshot that can be used to retrieve it.
	/// <exception cref="AlreadyHasRootSnapshotException">Thrown if this repository already has a root snapshot.
	/// Each repository can only have one root snapshot.</exception>
	ulong SaveRootSnapshot(T tree);

	/// Saves the given state tree with the given parent snapshot, then returns a hash of the snapshot that can be used to retrieve it.
	ulong SaveSnapshot(T tree, ulong parentHash);

	/// Retrieves the state tree identified by the given hash.
	/// <exception cref="HashNotFoundException">Thrown if the given hash does not exist.</exception>
	T GetSnapshot(ulong hash);

	/// Merges the two snapshots identified by the given source and target snapshots, returning the hash of the merged result.
	/// Conflict resolution is implementation specific.
	ulong MergeSnapshots(ulong targetSnapshotHash, ulong sourceSnapshotHash);

	/// Returns a hierarchical tree of snapshots starting from the root snapshot down through all branches.
	/// <exception cref="NoRootSnapshotException">Thrown if this repository does not have a root snapshot from which to "grow" the tree.</exception>
	SnapshotTree GetSnapshotTree();
}

/// <summary>
/// Represents a snapshot in the snapshot history and its children.
/// This stores this snapshot's hash as well as all of its SnapshotTree children.
/// Using this SnapshotTree's <paramref name="Hash"/> you can get the state tree at this snapshot using <see cref="IRepository{T}.GetSnapshot"/>.
/// </summary>
public record SnapshotTree(ulong Hash, ImmutableArray<SnapshotTree>? Children = null);
