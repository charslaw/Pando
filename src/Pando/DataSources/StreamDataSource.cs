using System;
using System.Collections.Generic;
using System.IO;
using Pando.DataSources.Utils;

namespace Pando.DataSources;

public class StreamDataSource : INodeDataSink, ISnapshotDataSink, IDisposable
{
	private readonly Stream _snapshotIndexStream;
	private readonly Stream _leafSnapshotsStream;
	private readonly Stream _nodeIndexStream;
	private readonly Stream _nodeDataStream;

	private readonly HashSet<SnapshotId> _leafSnapshotHashSet;

	/// Counts total number of bytes in the node data stream.
	private long _nodeDataBytesCount;

	public StreamDataSource(Stream snapshotIndexStream, Stream leafSnapshotsStream, Stream nodeIndexStream, Stream nodeDataStream)
	{
		_snapshotIndexStream = snapshotIndexStream;
		_leafSnapshotsStream = leafSnapshotsStream;
		_leafSnapshotHashSet = StreamUtils.LeafSnapshotSet.PopulateLeafSnapshotsSet(leafSnapshotsStream);
		_nodeIndexStream = nodeIndexStream;
		_nodeDataStream = nodeDataStream;
		_nodeDataBytesCount = nodeDataStream.Length;
	}

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
		_nodeDataStream.Write(bytes);
		_nodeDataBytesCount += bytes.Length;

		StreamUtils.NodeIndex.WriteIndexEntry(_nodeIndexStream, nodeId, (int)start, (int)_nodeDataBytesCount);
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
		StreamUtils.SnapshotIndex.WriteIndexEntry(_snapshotIndexStream, snapshotId, parentSnapshotId, rootNodeId);

		// Parent is by definition no longer a leaf node
		_leafSnapshotHashSet.Remove(parentSnapshotId);
		// Newly add snapshot is by definition a leaf node
		_leafSnapshotHashSet.Add(snapshotId);

		UpdateLeafSnapshotStream();
	}

	/// Overwrites the contents of the leaf snapshot stream with the current contents of the leaf snapshots set
	private void UpdateLeafSnapshotStream()
	{
		_leafSnapshotsStream.Seek(0, SeekOrigin.Begin);
		_leafSnapshotsStream.SetLength(0);
		Span<byte> buffer = stackalloc byte[_leafSnapshotHashSet.Count * SnapshotId.SIZE];
		var offset = 0;
		foreach (var leafId in _leafSnapshotHashSet)
		{
			leafId.CopyTo(buffer.Slice(offset, SnapshotId.SIZE));
			offset += SnapshotId.SIZE;
		}

		_leafSnapshotsStream.Write(buffer);
	}

	/// Disposes this StreamDataSource and all contained streams
	public void Dispose()
	{
		_snapshotIndexStream.Dispose();
		_leafSnapshotsStream.Dispose();
		_nodeIndexStream.Dispose();
		_nodeDataStream.Dispose();
	}
}
