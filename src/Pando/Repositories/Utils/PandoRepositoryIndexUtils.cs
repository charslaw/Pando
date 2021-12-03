using System;
using System.IO;

namespace Pando.Repositories.Utils;

internal static class PandoRepositoryIndexUtils
{
	private const int NODE_HASH_END = sizeof(ulong);
	private const int NODE_DATA_START_END = NODE_HASH_END + sizeof(int);
	private const int NODE_DATA_LEN_END = NODE_DATA_START_END + sizeof(int);
	private const int SIZE_OF_NODE_INDEX_ENTRY = NODE_DATA_LEN_END;

	/// Estimates the number of node index entries in the given stream.
	public static int GetNodeIndexEntryCount(Stream stream) => (int)stream.Length / SIZE_OF_NODE_INDEX_ENTRY;

	/// Translates the given node index details into the appropriate byte representation and writes it to this stream at the current position.
	public static void WriteNodeIndexEntry(Stream stream, ulong hash, int dataStart, int dataLength)
	{
		Span<byte> buffer = stackalloc byte[SIZE_OF_NODE_INDEX_ENTRY];
		ByteConverter.CopyBytes(hash, buffer[..NODE_HASH_END]);
		ByteConverter.CopyBytes(dataStart, buffer[NODE_HASH_END..NODE_DATA_START_END]);
		ByteConverter.CopyBytes(dataLength, buffer[NODE_DATA_START_END..NODE_DATA_LEN_END]);
		stream.Write(buffer);
	}

	/// Parses a node index hash and slice from this stream of bytes, starting from the current position.
	public static bool ReadNextNodeIndexEntry(Stream stream, out ulong hash, out DataSlice slice)
	{
		Span<byte> buffer = stackalloc byte[SIZE_OF_NODE_INDEX_ENTRY];
		var nBytesRead = stream.Read(buffer);
		switch (nBytesRead)
		{
			case 0:
				hash = default;
				slice = default;
				return false;
			case < SIZE_OF_NODE_INDEX_ENTRY:
				throw new IncompleteReadException("Could not read a full node index from the remaining bytes in the stream");
		}

		hash = ByteConverter.GetUInt64(buffer[..NODE_HASH_END]);
		var start = ByteConverter.GetInt32(buffer[NODE_HASH_END..NODE_DATA_START_END]);
		var length = ByteConverter.GetInt32(buffer[NODE_DATA_START_END..NODE_DATA_LEN_END]);
		slice = new DataSlice(start, length);
		return true;
	}

	private const int SS_HASH_END = sizeof(ulong);
	private const int SS_PARENT_HASH_END = SS_HASH_END + sizeof(ulong);
	private const int SS_ROOT_HASH_END = SS_PARENT_HASH_END + sizeof(ulong);
	private const int SIZE_OF_SNAPSHOT_INDEX_ENTRY = SS_ROOT_HASH_END;

	/// Estimates the number of node index entries in the given stream.
	public static int GetSnapshotIndexEntryCount(Stream stream) => (int)stream.Length / SIZE_OF_SNAPSHOT_INDEX_ENTRY;

	/// Translates the given snapshot index details into the appropriate byte representation and writes it to this stream at the current position.
	public static void WriteSnapshotIndexEntry(Stream stream, ulong hash, ulong parentHash, ulong rootNodeHash)
	{
		Span<byte> buffer = stackalloc byte[SIZE_OF_SNAPSHOT_INDEX_ENTRY];
		ByteConverter.CopyBytes(hash, buffer[..SS_HASH_END]);
		ByteConverter.CopyBytes(parentHash, buffer[SS_HASH_END..SS_PARENT_HASH_END]);
		ByteConverter.CopyBytes(rootNodeHash, buffer[SS_PARENT_HASH_END..SS_ROOT_HASH_END]);
		stream.Write(buffer);
	}

	/// Parses a snapshot hash and data from this stream of bytes, starting from the current position.
	public static bool ReadNextSnapshotIndexEntry(Stream stream, out ulong hash, out SnapshotData data)
	{
		Span<byte> buffer = stackalloc byte[SIZE_OF_SNAPSHOT_INDEX_ENTRY];
		var nBytesRead = stream.Read(buffer);
		switch (nBytesRead)
		{
			case 0:
				hash = default;
				data = default;
				return false;
			case < SIZE_OF_SNAPSHOT_INDEX_ENTRY:
				throw new IncompleteReadException("Could not read a full snapshot index from the remaining bytes in the stream");
		}

		hash = ByteConverter.GetUInt64(buffer[..SS_HASH_END]);
		var parentHash = ByteConverter.GetUInt64(buffer[SS_HASH_END..SS_PARENT_HASH_END]);
		var rootNodeHash = ByteConverter.GetUInt64(buffer[SS_PARENT_HASH_END..SS_ROOT_HASH_END]);
		data = new SnapshotData(parentHash, rootNodeHash);
		return true;
	}
}

internal class IncompleteReadException : Exception
{
	public IncompleteReadException(string message) : base(message) { }
}
