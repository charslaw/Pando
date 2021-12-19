using System;
using System.IO;
using Pando.Repositories.Utils;

namespace Pando.Repositories;

public class StreamRepository : IWritablePandoNodeRepository, IWritablePandoSnapshotRepository, IDisposable
{
	private readonly Stream _snapshotIndexStream;
	private readonly Stream _nodeIndexStream;
	private readonly Stream _nodeDataStream;

	/// Counts total number of bytes in the node data stream.
	private long _nodeDataBytesCount;

	public StreamRepository(Stream snapshotIndexStream, Stream nodeIndexStream, Stream nodeDataStream)
	{
		_snapshotIndexStream = snapshotIndexStream;
		_nodeIndexStream = nodeIndexStream;
		_nodeDataStream = nodeDataStream;
		_nodeDataBytesCount = nodeDataStream.Length;
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
		_nodeDataStream.Write(bytes.ToArray(), 0, bytes.Length);
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
	}

	/// Disposes this StreamRepository and all contained streams
	public void Dispose()
	{
		_snapshotIndexStream.Dispose();
		_nodeIndexStream.Dispose();
		_nodeDataStream.Dispose();
	}
}
