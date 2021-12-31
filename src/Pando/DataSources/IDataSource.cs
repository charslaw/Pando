using System;
using System.Collections.Immutable;
using Pando.Exceptions;

namespace Pando.DataSources;

public interface INodeDataSink
{
	/// Adds a node to the data sink and returns a hash that can be used to retrieve the node.
	ulong AddNode(ReadOnlySpan<byte> bytes);
}

public interface ISnapshotDataSink
{
	/// Adds a snapshot pointing to a node identified by the given hash with a parent snapshot identified by the given hash.
	/// <remarks>A root snapshot (i.e. a snapshot with no parent snapshot)
	/// should have a <paramref name="parentHash"/> of <c>0UL</c>.</remarks>
	ulong AddSnapshot(ulong parentHash, ulong rootNodeHash);
}

public interface INodeDataSource
{
	/// Returns whether a node identified by the given hash exists in the data source.
	bool HasNode(ulong hash);

	/// Gets the size in bytes of the node identified by the given hash.
	int GetSizeOfNode(ulong hash);

	void CopyNodeBytesTo(ulong hash, ref Span<byte> outputBytes);
}

public interface ISnapshotDataSource
{
	/// The total number of snapshots in this data source.
	int SnapshotCount { get; }

	/// Returns whether a snapshot identified by the given hash exists in the data source.
	bool HasSnapshot(ulong hash);

	/// Returns the hash of the parent of the snapshot identified by the given hash.
	/// <exception cref="HashNotFoundException">thrown if there is no snapshot node identified by the given hash.</exception>
	ulong GetSnapshotParent(ulong hash);

	/// Returns the hash of the root Node of the snapshot identified by the given hash.
	/// <exception cref="HashNotFoundException">thrown if there is no snapshot node identified by the given hash.</exception>
	ulong GetSnapshotRootNode(ulong hash);

	/// Returns a set of snapshot hashes that correspond with the "heads" of each branch in the snapshot tree in this data source.
	IImmutableSet<ulong> GetLeafSnapshotHashes();
}

/// Stores data Snapshots and Nodes
public interface IDataSource
	: INodeDataSink, ISnapshotDataSink, INodeDataSource, ISnapshotDataSource { }
