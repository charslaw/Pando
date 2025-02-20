using System;
using System.Collections.Generic;
using Pando.DataSources.Utils;
using Pando.Exceptions;

namespace Pando.DataSources;

public interface INodeDataSink
{
	/// Adds a node to the data sink and returns a <see cref="NodeId"/> that can be used to retrieve the node.
	NodeId AddNode(ReadOnlySpan<byte> bytes);
}

public interface ISnapshotDataSink
{
	/// Adds a snapshot pointing to a node identified by the given <see cref="NodeId"/> with a parent snapshot identified by the given <see cref="SnapshotId"/>.
	/// <remarks>A root snapshot (i.e. a snapshot with no parent snapshot)
	/// should have a <paramref name="parentSnapshotId"/> of <see cref="SnapshotId.None"/>.</remarks>
	SnapshotId AddSnapshot(SnapshotId parentSnapshotId, NodeId rootNodeId);
}

public interface INodeDataSource
{
	/// Returns whether a node identified by the given <see cref="NodeId"/> exists in the data source.
	bool HasNode(NodeId nodeId);

	/// Gets the size in bytes of the node identified by the given <see cref="NodeId"/>.
	/// <exception cref="NodeIdNotFoundException">If the given <paramref name="nodeId"/> is not found in the data source.</exception>
	int GetSizeOfNode(NodeId nodeId);

	/// Copies the binary representation of the node with the given <see cref="NodeId"/> into the given Span.
	/// <exception cref="NodeIdNotFoundException">If the given <paramref name="nodeId"/> is not found in the data source.</exception>
	/// <exception cref="ArgumentOutOfRangeException">If the given span is not large enough to contain the node data.</exception>
	void CopyNodeBytesTo(NodeId nodeId, Span<byte> outputBytes);
}

public interface ISnapshotDataSource
{
	/// The total number of snapshots in this data source.
	int SnapshotCount { get; }

	/// Returns whether a snapshot identified by the given <see cref="SnapshotId"/> exists in the data source.
	bool HasSnapshot(SnapshotId snapshotId);

	/// Returns the <see cref="SnapshotId"/> of the parent of the snapshot identified by the given <see cref="SnapshotId"/>.
	/// <exception cref="SnapshotIdNotFound">thrown if there is no snapshot node identified by the given <see cref="SnapshotId"/>.</exception>
	SnapshotId GetSnapshotParent(SnapshotId snapshotId);

	/// Returns the <see cref="SnapshotId"/> of the least common ancestor snapshot of the two snapshots identified by the given <see cref="SnapshotId"/>s.
	SnapshotId GetSnapshotLeastCommonAncestor(SnapshotId id1, SnapshotId id2);

	/// Returns the <see cref="SnapshotId"/> of the root Node of the snapshot identified by the given <see cref="SnapshotId"/>.
	/// <exception cref="HashNotFoundException">thrown if there is no snapshot node identified by the given <see cref="SnapshotId"/>.</exception>
	NodeId GetSnapshotRootNode(SnapshotId snapshotId);
}

/// Stores data Snapshots and Nodes
public interface IDataSource : INodeDataSink, ISnapshotDataSink, INodeDataSource, ISnapshotDataSource;
