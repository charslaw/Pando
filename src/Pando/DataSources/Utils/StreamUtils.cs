using System;
using System.Collections.Generic;
using System.IO;

namespace Pando.DataSources.Utils;

internal static class StreamUtils
{
	internal static class NodeIndex
	{
		private const int NODE_HASH_END = NodeId.SIZE;
		private const int NODE_DATA_START_END = NODE_HASH_END + sizeof(int);
		private const int NODE_DATA_END_END = NODE_DATA_START_END + sizeof(int);
		private const int SIZE_OF_NODE_INDEX_ENTRY = NODE_DATA_END_END;

		/// Translates the given node index details into the appropriate byte representation and writes it to this stream at the current position.
		public static void WriteIndexEntry(Stream stream, NodeId nodeId, int dataStart, int dataEnd)
		{
			Span<byte> buffer = stackalloc byte[SIZE_OF_NODE_INDEX_ENTRY];
			nodeId.CopyTo(buffer[..NODE_HASH_END]);
			ByteEncoder.CopyBytes(dataStart, buffer[NODE_HASH_END..NODE_DATA_START_END]);
			ByteEncoder.CopyBytes(dataEnd, buffer[NODE_DATA_START_END..NODE_DATA_END_END]);
			stream.Write(buffer);
		}

		public static Dictionary<NodeId, Range> PopulateNodeIndex(Stream nodeIndexStream, Dictionary<NodeId, Range>? index)
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
			index ??= new Dictionary<NodeId, Range>(totalEntriesCount);
			if (totalEntriesCount == 0) return index;

			Span<byte> hashBuffer = stackalloc byte[SIZE_OF_NODE_INDEX_ENTRY];
			for (int i = 0; i < totalEntriesCount; i++)
			{
				nodeIndexStream.ReadExactly(hashBuffer);
				var hash = NodeId.FromBuffer(hashBuffer[..NODE_HASH_END]);
				var dataStart = ByteEncoder.GetInt32(hashBuffer[NODE_HASH_END..NODE_DATA_START_END]);
				var dataEnd = ByteEncoder.GetInt32(hashBuffer[NODE_DATA_START_END..NODE_DATA_END_END]);
				var snapshotData = (dataStart..dataEnd);

				index.Add(hash, snapshotData);
			}

			return index;
		}
	}

	internal static class SnapshotIndex
	{
		private const int SS_HASH_END = SnapshotId.SIZE;
		private const int SS_PARENT_HASH_END = SS_HASH_END + SnapshotId.SIZE;
		private const int SS_ROOT_HASH_END = SS_PARENT_HASH_END + NodeId.SIZE;
		private const int SIZE_OF_SNAPSHOT_INDEX_ENTRY = SS_ROOT_HASH_END;

		/// Translates the given snapshot index details into the appropriate byte representation and writes it to this stream at the current position.
		public static void WriteIndexEntry(Stream stream, SnapshotId snapshotId, SnapshotId parentHash, NodeId rootNodeHash)
		{
			Span<byte> buffer = stackalloc byte[SIZE_OF_SNAPSHOT_INDEX_ENTRY];
			snapshotId.CopyTo(buffer[..SS_HASH_END]);
			parentHash.CopyTo(buffer[SS_HASH_END..SS_PARENT_HASH_END]);
			rootNodeHash.CopyTo(buffer[SS_PARENT_HASH_END..SS_ROOT_HASH_END]);
			stream.Write(buffer);
		}

		public static Dictionary<SnapshotId, SnapshotData> PopulateSnapshotIndex(Stream snapshotIndexStream, Dictionary<SnapshotId, SnapshotData>? index)
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
			index ??= new Dictionary<SnapshotId, SnapshotData>(totalEntriesCount);
			if (totalEntriesCount == 0) return index;

			Span<byte> hashBuffer = stackalloc byte[SIZE_OF_SNAPSHOT_INDEX_ENTRY];
			for (int i = 0; i < totalEntriesCount; i++)
			{
				snapshotIndexStream.ReadExactly(hashBuffer);
				var hash = SnapshotId.FromBuffer(hashBuffer[..SS_HASH_END]);
				var parentHash = SnapshotId.FromBuffer(hashBuffer[SS_HASH_END..SS_PARENT_HASH_END]);
				var rootNodeHash = NodeId.FromBuffer(hashBuffer[SS_PARENT_HASH_END..SS_ROOT_HASH_END]);
				var snapshotData = new SnapshotData(parentHash, rootNodeHash);

				index.Add(hash, snapshotData);
			}

			return index;
		}
	}

	internal static class LeafSnapshotSet
	{
		public static HashSet<SnapshotId> PopulateLeafSnapshotsSet(Stream leafSnapshotsStream)
		{
			leafSnapshotsStream.Seek(0, SeekOrigin.Begin);
			if (leafSnapshotsStream.Length % SnapshotId.SIZE != 0)
			{
				throw new IncompleteReadException(
					$"{nameof(leafSnapshotsStream)} has a wrong number of bytes: {leafSnapshotsStream.Length}." +
					$"Length must be a multiple of {SnapshotId.SIZE}."
				);
			}

			var totalHashesCount = (int)leafSnapshotsStream.Length / SnapshotId.SIZE;
			if (totalHashesCount == 0) return new HashSet<SnapshotId>();

			var set = new HashSet<SnapshotId>(totalHashesCount);
			Span<byte> idBuffer = stackalloc byte[SnapshotId.SIZE];
			for (int i = 0; i < totalHashesCount; i++)
			{
				leafSnapshotsStream.ReadExactly(idBuffer);
				set.Add(SnapshotId.FromBuffer(idBuffer));
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
