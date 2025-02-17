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

	/// <summary>
	/// Creates a new snapshot that merges the snapshots identified by the given parent hashes, then returns a hash of the merged snapshot that
	/// can be used to retrieve it.
	/// The behavior of the merge in the case of conflicts is implementation specific.
	/// </summary>
	ulong MergeSnapshots(ulong targetHash, ulong sourceHash);

	/// Retrieves the state tree identified by the given hash.
	/// <exception cref="HashNotFoundException">Thrown if the given hash does not exist.</exception>
	T GetSnapshot(ulong hash);

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
