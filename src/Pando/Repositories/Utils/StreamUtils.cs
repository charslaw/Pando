using System;
using System.Collections.Generic;
using System.IO;

namespace Pando.Repositories.Utils;

internal static class StreamUtils
{
	internal static class NodeIndex
	{
		private const int NODE_HASH_END = sizeof(ulong);
		private const int NODE_DATA_START_END = NODE_HASH_END + sizeof(int);
		private const int NODE_DATA_LEN_END = NODE_DATA_START_END + sizeof(int);
		private const int SIZE_OF_NODE_INDEX_ENTRY = NODE_DATA_LEN_END;

		/// Translates the given node index details into the appropriate byte representation and writes it to this stream at the current position.
		public static void WriteIndexEntry(Stream stream, ulong hash, int dataStart, int dataLength)
		{
			Span<byte> buffer = stackalloc byte[SIZE_OF_NODE_INDEX_ENTRY];
			ByteEncoder.CopyBytes(hash, buffer[..NODE_HASH_END]);
			ByteEncoder.CopyBytes(dataStart, buffer[NODE_HASH_END..NODE_DATA_START_END]);
			ByteEncoder.CopyBytes(dataLength, buffer[NODE_DATA_START_END..NODE_DATA_LEN_END]);
			stream.Write(buffer);
		}

		public static Dictionary<ulong, DataSlice> PopulateNodeIndex(Stream nodeIndexStream, Dictionary<ulong, DataSlice>? index)
		{
			nodeIndexStream.Seek(0, SeekOrigin.Begin);
			if (nodeIndexStream.Length % SIZE_OF_NODE_INDEX_ENTRY != 0)
			{
				throw new IncompleteReadException(
					$"{nameof(nodeIndexStream)} has a wrong number of bytes: {nodeIndexStream.Length}." +
					$"Length must be a multiple of {SIZE_OF_NODE_INDEX_ENTRY}."
				);
			}

			var totalEntriesCount = (int)nodeIndexStream.Length / SIZE_OF_NODE_INDEX_ENTRY;
			index ??= new Dictionary<ulong, DataSlice>(totalEntriesCount);
			if (totalEntriesCount == 0) return index;

			Span<byte> hashBuffer = stackalloc byte[SIZE_OF_NODE_INDEX_ENTRY];
			for (int i = 0; i < totalEntriesCount; i++)
			{
				nodeIndexStream.Read(hashBuffer);
				var hash = ByteEncoder.GetUInt64(hashBuffer[..NODE_HASH_END]);
				var parentHash = ByteEncoder.GetInt32(hashBuffer[NODE_HASH_END..NODE_DATA_START_END]);
				var rootNodeHash = ByteEncoder.GetInt32(hashBuffer[NODE_DATA_START_END..NODE_DATA_LEN_END]);
				var snapshotData = new DataSlice(parentHash, rootNodeHash);

				index.Add(hash, snapshotData);
			}

			return index;
		}
	}

	internal static class SnapshotIndex
	{
		private const int SS_HASH_END = sizeof(ulong);
		private const int SS_PARENT_HASH_END = SS_HASH_END + sizeof(ulong);
		private const int SS_ROOT_HASH_END = SS_PARENT_HASH_END + sizeof(ulong);
		private const int SIZE_OF_SNAPSHOT_INDEX_ENTRY = SS_ROOT_HASH_END;

		/// Translates the given snapshot index details into the appropriate byte representation and writes it to this stream at the current position.
		public static void WriteIndexEntry(Stream stream, ulong hash, ulong parentHash, ulong rootNodeHash)
		{
			Span<byte> buffer = stackalloc byte[SIZE_OF_SNAPSHOT_INDEX_ENTRY];
			ByteEncoder.CopyBytes(hash, buffer[..SS_HASH_END]);
			ByteEncoder.CopyBytes(parentHash, buffer[SS_HASH_END..SS_PARENT_HASH_END]);
			ByteEncoder.CopyBytes(rootNodeHash, buffer[SS_PARENT_HASH_END..SS_ROOT_HASH_END]);
			stream.Write(buffer);
		}

		public static Dictionary<ulong, SnapshotData> PopulateSnapshotIndex(Stream snapshotIndexStream, Dictionary<ulong, SnapshotData>? index)
		{
			snapshotIndexStream.Seek(0, SeekOrigin.Begin);
			if (snapshotIndexStream.Length % SIZE_OF_SNAPSHOT_INDEX_ENTRY != 0)
			{
				throw new IncompleteReadException(
					$"{nameof(snapshotIndexStream)} has a wrong number of bytes: {snapshotIndexStream.Length}." +
					$"Length must be a multiple of {SIZE_OF_SNAPSHOT_INDEX_ENTRY}."
				);
			}

			var totalEntriesCount = (int)snapshotIndexStream.Length / SIZE_OF_SNAPSHOT_INDEX_ENTRY;
			index ??= new Dictionary<ulong, SnapshotData>(totalEntriesCount);
			if (totalEntriesCount == 0) return index;

			Span<byte> hashBuffer = stackalloc byte[SIZE_OF_SNAPSHOT_INDEX_ENTRY];
			for (int i = 0; i < totalEntriesCount; i++)
			{
				snapshotIndexStream.Read(hashBuffer);
				var hash = ByteEncoder.GetUInt64(hashBuffer[..SS_HASH_END]);
				var parentHash = ByteEncoder.GetUInt64(hashBuffer[SS_HASH_END..SS_PARENT_HASH_END]);
				var rootNodeHash = ByteEncoder.GetUInt64(hashBuffer[SS_PARENT_HASH_END..SS_ROOT_HASH_END]);
				var snapshotData = new SnapshotData(parentHash, rootNodeHash);

				index.Add(hash, snapshotData);
			}

			return index;
		}
	}

	internal static class LeafSnapshotSet
	{
		private const int SIZE_OF_SNAPSHOT_HASH = sizeof(ulong);

		public static HashSet<ulong> PopulateLeafSnapshotsSet(Stream leafSnapshotsStream)
		{
			leafSnapshotsStream.Seek(0, SeekOrigin.Begin);
			if (leafSnapshotsStream.Length % SIZE_OF_SNAPSHOT_HASH != 0)
			{
				throw new IncompleteReadException(
					$"{nameof(leafSnapshotsStream)} has a wrong number of bytes: {leafSnapshotsStream.Length}." +
					$"Length must be a multiple of {SIZE_OF_SNAPSHOT_HASH}."
				);
			}

			var totalHashesCount = (int)leafSnapshotsStream.Length / SIZE_OF_SNAPSHOT_HASH;
			if (totalHashesCount == 0) return new HashSet<ulong>();

			var set = new HashSet<ulong>(totalHashesCount);
			Span<byte> hashBuffer = stackalloc byte[SIZE_OF_SNAPSHOT_HASH];
			for (int i = 0; i < totalHashesCount; i++)
			{
				leafSnapshotsStream.Read(hashBuffer);
				set.Add(ByteEncoder.GetUInt64(hashBuffer));
			}

			return set;
		}
	}

	internal static class NodeData
	{
		public static SpannableList<byte> PopulateNodeData(Stream nodeDataStream, SpannableList<byte>? data)
		{
			const int MAX_BUFFER_SIZE = 500 * 1000; // 500 KB

			nodeDataStream.Seek(0, SeekOrigin.Begin);
			var streamLength = (int)nodeDataStream.Length;
			data ??= new SpannableList<byte>(streamLength);
			if (streamLength <= 0) return data;

			Span<byte> buffer = stackalloc byte[Math.Min(streamLength, MAX_BUFFER_SIZE)];

			var totalChunks = ((streamLength - 1) / MAX_BUFFER_SIZE) + 1; // Math.Ceiling(streamLength / MAX_BUFFER_SIZE) for ints

			for (int i = 0; i < totalChunks; i++)
			{
				var bytesRead = nodeDataStream.Read(buffer);
				data.AddSpan(buffer[..bytesRead]);
			}

			return data;
		}
	}
}

internal class IncompleteReadException : Exception
{
	public IncompleteReadException(string message) : base(message) { }
}
