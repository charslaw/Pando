using System.Collections.Generic;
using Pando.DataSources.Utils;
using Pando.Exceptions;

namespace Pando.DataStructures;

/// Contains the parent ids of a snapshot in the tree.
/// <remarks>In the standard case, only the <paramref name="SourceParentSnapshotId"/> will be populated.
/// The <paramref name="TargetParentSnapshotId"/> will be populated in the case of a merge snapshot.</remarks>
/// <param name="SourceParentSnapshotId">The "primary" parent of the child snapshot.</param>
/// <param name="TargetParentSnapshotId">The "secondary" parent of the child snapshot. If the child snapshot is *not* a merge snapshot,
/// this will be <see cref="SnapshotId.None"/>.</param>
public record struct SnapshotParents(SnapshotId SourceParentSnapshotId, SnapshotId TargetParentSnapshotId);

public interface IReadOnlySnapshotTree
{
	/// Returns the root snapshot of the tree.
	public SnapshotId? RootSnapshot { get; }

	/// Returns whether the given snapshot exists in the tree.
	public bool HasSnapshot(SnapshotId snapshotId);

	/// Enumerates the child ids of the snapshot with the given id.
	public IEnumerable<SnapshotId> GetSnapshotChildren(SnapshotId snapshotId);

	/// Returns the parent snapshot id(s) of the snapshot with the given id.
	public SnapshotParents GetSnapshotParents(SnapshotId snapshotId);

	/// Walks through the tree, depth first, in order of original insertion into the tree.
	public IEnumerable<(SnapshotId snapshotId, SnapshotParents parents)> EnumerateDepthFirst();
}

public interface ISnapshotTree : IReadOnlySnapshotTree
{
	/// Adds a root snapshot to the tree.
	/// <exception cref="AlreadyHasRootSnapshotException">if a root snapshot already exists in the tree.</exception>
	public void AddRootSnapshot(SnapshotId rootSnapshotId);

	/// Adds a snapshot to the tree with the given parents.
	public void AddSnapshot(SnapshotId snapshotId, SnapshotParents parents);

	/// Adds a snapshot to the tree with the given source parent (non merged).
	public void AddSnapshot(SnapshotId snapshotId, SnapshotId parentSnapshotId);
}
