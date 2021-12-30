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

	private readonly HashSet<ulong> _leafSnapshotHashSet;

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
	public ulong AddNode(ReadOnlySpan<byte> bytes)
	{
		var hash = HashUtils.ComputeNodeHash(bytes);
		AddNodeWithHashUnsafe(hash, bytes);
		return hash;
	}

	/// Adds a new node indexed with the given hash, containing the given bytes
	/// <remarks>
	///     <para>This method is unsafe because the given hash and data might mismatch.
	///     When calling this method, ensure the correct hash is given.</para>
	///     <para>The StreamDataSource <i>does not</i> defend against duplicate nodes.
	///     Before adding a node, you should ensure it is not a duplicate.</para>
	/// </remarks>
	internal void AddNodeWithHashUnsafe(ulong hash, ReadOnlySpan<byte> bytes)
	{
		var start = _nodeDataBytesCount;
		_nodeDataStream.Write(bytes);
		_nodeDataBytesCount += bytes.Length;

		StreamUtils.NodeIndex.WriteIndexEntry(_nodeIndexStream, hash, (int)start, bytes.Length);
	}

	/// <remarks>The StreamDataSource <i>does not</i> defend against duplicate snapshots.
	/// Before adding a snapshot, you should ensure it is not a duplicate.</remarks>
	/// <inheritdoc/>
	public ulong AddSnapshot(ulong parentHash, ulong rootNodeHash)
	{
		var hash = HashUtils.ComputeSnapshotHash(parentHash, rootNodeHash);
		AddSnapshotWithHashUnsafe(hash, parentHash, rootNodeHash);
		return hash;
	}

	/// Adds a new snapshot indexed with the given hash, containing the parent hash and node hash
	/// <remarks>
	///     <para>This method is unsafe because the given hash and data might mismatch.
	///     When calling this method, ensure the correct hash is given.</para>
	///     <para>The StreamDataSource <i>does not</i> defend against duplicate snapshots.
	///     Before adding a snapshot, you should ensure it is not a duplicate.</para>
	/// </remarks>
	internal void AddSnapshotWithHashUnsafe(ulong hash, ulong parentHash, ulong rootNodeHash)
	{
		StreamUtils.SnapshotIndex.WriteIndexEntry(_snapshotIndexStream, hash, parentHash, rootNodeHash);

		// Parent is by definition no longer a leaf node
		_leafSnapshotHashSet.Remove(parentHash);
		// Newly add snapshot is by definition a leaf node
		_leafSnapshotHashSet.Add(hash);

		UpdateLeafSnapshotStream();
	}

	/// Overwrites the contents of the leaf snapshot stream with the current contents of the leaf snapshots set
	private void UpdateLeafSnapshotStream()
	{
		_leafSnapshotsStream.Seek(0, SeekOrigin.Begin);
		_leafSnapshotsStream.SetLength(0);
		Span<byte> buffer = stackalloc byte[_leafSnapshotHashSet.Count * sizeof(ulong)];
		var offset = 0;
		foreach (var leafHash in _leafSnapshotHashSet)
		{
			ByteEncoder.CopyBytes(leafHash, buffer.Slice(offset, sizeof(ulong)));
			offset += sizeof(ulong);
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
