using System;
using System.IO;
using Pando.DataSources.Utils;

namespace Pando.DataSources;

public class StreamAppendingDataSink(Stream snapshotIndexStream, Stream nodeIndexStream, Stream nodeDataStream)
	: INodeDataSink, ISnapshotDataSink, IDisposable
{
	/// Counts total number of bytes in the node data stream.
	private long _nodeDataBytesCount = nodeDataStream.Length;

	/// <remarks>The StreamDataSource <i>does not</i> defend against duplicate nodes.
	/// Before adding a node, you should ensure it is not a duplicate</remarks>
	/// <inheritdoc/>
	public NodeId AddNode(ReadOnlySpan<byte> bytes)
	{
		var nodeId = HashUtils.ComputeNodeHash(bytes);
		AddNodeWithHashUnsafe(nodeId, bytes);
		return nodeId;
	}

	/// Adds a new node indexed with the given hash, containing the given bytes
	/// <remarks>
	///     <para>This method is unsafe because the given hash and data might mismatch.
	///     When calling this method, ensure the correct hash is given.</para>
	///     <para>The StreamDataSource <i>does not</i> defend against duplicate nodes.
	///     Before adding a node, you should ensure it is not a duplicate.</para>
	/// </remarks>
	internal void AddNodeWithHashUnsafe(NodeId nodeId, ReadOnlySpan<byte> bytes)
	{
		var start = _nodeDataBytesCount;
		nodeDataStream.Write(bytes);
		_nodeDataBytesCount += bytes.Length;

		StreamUtils.NodeIndex.WriteIndexEntry(nodeIndexStream, nodeId, (int)start, (int)_nodeDataBytesCount);
	}

	/// <remarks>The StreamDataSource <i>does not</i> defend against duplicate snapshots.
	/// Before adding a snapshot, you should ensure it is not a duplicate.</remarks>
	/// <inheritdoc/>
	public SnapshotId AddSnapshot(SnapshotId parentSnapshotId, NodeId rootNodeId)
	{
		var snapshotId = HashUtils.ComputeSnapshotHash(parentSnapshotId, rootNodeId);
		AddSnapshotWithHashUnsafe(snapshotId, parentSnapshotId, rootNodeId);
		return snapshotId;
	}

	/// Adds a new snapshot indexed with the given hash, containing the parent hash and node hash
	/// <remarks>
	///     <para>This method is unsafe because the given hash and data might mismatch.
	///     When calling this method, ensure the correct hash is given.</para>
	///     <para>The StreamDataSource <i>does not</i> defend against duplicate snapshots.
	///     Before adding a snapshot, you should ensure it is not a duplicate.</para>
	/// </remarks>
	internal void AddSnapshotWithHashUnsafe(SnapshotId snapshotId, SnapshotId parentSnapshotId, NodeId rootNodeId)
	{
		StreamUtils.SnapshotIndex.WriteIndexEntry(snapshotIndexStream, snapshotId, parentSnapshotId, rootNodeId);
	}

	/// Disposes this StreamDataSource and all contained streams
	public void Dispose()
	{
		snapshotIndexStream.Dispose();
		nodeIndexStream.Dispose();
		nodeDataStream.Dispose();
	}
}
