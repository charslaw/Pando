using System;
using System.Collections.Generic;
using System.IO;
using Pando.Repositories.Utils;

namespace Pando.Repositories;

public class StreamRepository : IWritablePandoNodeRepository, IWritablePandoSnapshotRepository, IDisposable
{
	private readonly Stream _snapshotIndexStream;
	private readonly Stream _leafSnapshotsStream;
	private readonly Stream _nodeIndexStream;
	private readonly Stream _nodeDataStream;

	private readonly HashSet<ulong> _leafSnapshotHashSet;

	/// Counts total number of bytes in the node data stream.
	private long _nodeDataBytesCount;

	public StreamRepository(Stream snapshotIndexStream, Stream leafSnapshotsStream, Stream nodeIndexStream, Stream nodeDataStream)
	{
		_snapshotIndexStream = snapshotIndexStream;
		_leafSnapshotsStream = leafSnapshotsStream;
		_leafSnapshotHashSet = GetInitialLeafSnapshots(leafSnapshotsStream);
		_nodeIndexStream = nodeIndexStream;
		_nodeDataStream = nodeDataStream;
		_nodeDataBytesCount = nodeDataStream.Length;
	}

	private static HashSet<ulong> GetInitialLeafSnapshots(Stream leafSnapshotsStream)
	{
		leafSnapshotsStream.Seek(0, SeekOrigin.Begin);
		if (leafSnapshotsStream.Length % 8 != 0)
		{
			throw new IncompleteReadException(
				$"{nameof(leafSnapshotsStream)} has a wrong number of bytes: {leafSnapshotsStream.Length}." +
				"Length must be a multiple of 8."
			);
		}

		var totalHashesCount = (int)leafSnapshotsStream.Length / 8;
		if (totalHashesCount == 0) return new HashSet<ulong>();

		var set = new HashSet<ulong>(totalHashesCount);
		Span<byte> hashBuffer = stackalloc byte[sizeof(ulong)];
		for (int i = 0; i < totalHashesCount; i++)
		{
			leafSnapshotsStream.Read(hashBuffer);
			set.Add(ByteEncoder.GetUInt64(hashBuffer));
		}

		return set;
	}

	/// <remarks>The StreamRepository <i>does not</i> defend against duplicate nodes.
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
	///     <para>The StreamRepository <i>does not</i> defend against duplicate nodes.
	///     Before adding a node, you should ensure it is not a duplicate.</para>
	/// </remarks>
	internal void AddNodeWithHashUnsafe(ulong hash, ReadOnlySpan<byte> bytes)
	{
		var start = _nodeDataBytesCount;
		_nodeDataStream.Write(bytes);
		_nodeDataBytesCount += bytes.Length;

		NodeIndexUtils.WriteIndexEntry(_nodeIndexStream, hash, (int)start, bytes.Length);
	}

	/// <remarks>The StreamRepository <i>does not</i> defend against duplicate snapshots.
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
	///     <para>The StreamRepository <i>does not</i> defend against duplicate snapshots.
	///     Before adding a snapshot, you should ensure it is not a duplicate.</para>
	/// </remarks>
	internal void AddSnapshotWithHashUnsafe(ulong hash, ulong parentHash, ulong rootNodeHash)
	{
		SnapshotIndexUtils.WriteIndexEntry(_snapshotIndexStream, hash, parentHash, rootNodeHash);

		_leafSnapshotHashSet.Remove(parentHash);
		_leafSnapshotHashSet.Add(hash);

		_leafSnapshotsStream.Seek(0, SeekOrigin.Begin);
		_leafSnapshotsStream.SetLength(0);
		Span<byte> buffer = stackalloc byte[_leafSnapshotHashSet.Count * sizeof(ulong)];
		var i = 0;
		foreach (var leafHash in _leafSnapshotHashSet)
		{
			ByteEncoder.CopyBytes(leafHash, buffer.Slice(i * sizeof(ulong), sizeof(ulong)));
			i++;
		}

		_leafSnapshotsStream.Write(buffer);
	}

	/// Disposes this StreamRepository and all contained streams
	public void Dispose()
	{
		_snapshotIndexStream.Dispose();
		_leafSnapshotsStream.Dispose();
		_nodeIndexStream.Dispose();
		_nodeDataStream.Dispose();
	}
}
