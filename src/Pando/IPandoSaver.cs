using System.Collections.Immutable;
using Pando.Exceptions;

namespace Pando;

public interface IPandoSaver<T>
{
	/// Saves the given state tree snapshot as a root snapshot, then returns a hash of the snapshot that can be used to retrieve it.
	/// <exception cref="AlreadyHasRootSnapshotException">Thrown if this PandoSaver already has a root snapshot.
	/// Each PandoSaver can only have one root snapshot.</exception>
	ulong SaveRootSnapshot(T tree);

	/// Saves the given state tree with the given parent snapshot, then returns a hash of the snapshot that can be used to retrieve it.
	ulong SaveSnapshot(T tree, ulong parentHash);

	/// Retrieves the state tree identified by the given hash.
	/// <exception cref="HashNotFoundException">Thrown if the given hash does not exist.</exception>
	T GetSnapshot(ulong hash);

	SnapshotTree GetSnapshotTree();
}

public record SnapshotTree(ulong Hash, ImmutableArray<SnapshotTree>? Children = null);
