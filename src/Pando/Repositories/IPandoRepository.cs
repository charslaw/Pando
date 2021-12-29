using System;
using System.Collections.Immutable;
using Pando.Exceptions;

namespace Pando.Repositories;

public interface IWritablePandoNodeRepository
{
	/// Adds a node to the repository and returns a hash that can be used to retrieve the node.
	ulong AddNode(ReadOnlySpan<byte> bytes);
}

public interface IWritablePandoSnapshotRepository
{
	/// Adds a snapshot pointing to a node identified by the given hash with a parent snapshot identified by the given hash.
	/// <remarks>A root snapshot (i.e. a snapshot with no parent snapshot)
	/// should have a <paramref name="parentHash"/> of <c>0UL</c>.</remarks>
	ulong AddSnapshot(ulong parentHash, ulong rootNodeHash);
}

public interface IReadablePandoNodeRepository
{
	/// Returns whether a node identified by the given hash exists in the repository
	bool HasNode(ulong hash);

	/// Retrieves the data for the node identified by the given hash,
	/// then delegates deserialization to the given <paramref name="nodeDeserializer"/>, then returns the result.
	/// <exception cref="HashNotFoundException">thrown if there is no node identified by the given hash.</exception>
	/// <returns>The deserialized object, as returned by the <paramref name="nodeDeserializer"/></returns>
	T GetNode<T>(ulong hash, in IPandoNodeDeserializer<T> nodeDeserializer);

	/// Gets the size in bytes of the node identified by the given hash.
	int GetSizeOfNode(ulong hash);
}

public interface IReadablePandoSnapshotRepository
{
	/// The total number of snapshots in this repository
	int SnapshotCount { get; }

	/// Returns whether a snapshot identified by the given hash exists in the repository
	bool HasSnapshot(ulong hash);

	/// Returns the hash of the parent of the snapshot identified by the given hash.
	/// <exception cref="HashNotFoundException">thrown if there is no snapshot node identified by the given hash.</exception>
	ulong GetSnapshotParent(ulong hash);

	/// Returns the hash of the root Node of the snapshot identified by the given hash.
	/// <exception cref="HashNotFoundException">thrown if there is no snapshot node identified by the given hash.</exception>
	ulong GetSnapshotRootNode(ulong hash);

	/// Returns a set of snapshot hashes that correspond with the "heads" of each branch in the snapshot tree in this repository.
	IImmutableSet<ulong> GetLeafSnapshotHashes();
}

/// Stores data Snapshots and Nodes
public interface IPandoRepository
	: IWritablePandoNodeRepository, IWritablePandoSnapshotRepository, IReadablePandoNodeRepository, IReadablePandoSnapshotRepository { }
