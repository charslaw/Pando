using System;
using System.IO;

namespace Pando.Repositories.Utils
{
    internal static class PandoRepositoryIndexUtils
    {
        private const int SIZE_OF_NODE_INDEX_ENTRY = sizeof(ulong) + sizeof(int) + sizeof(int);
        private static readonly byte[] nodeIndexBuffer = new byte[SIZE_OF_NODE_INDEX_ENTRY];

        /// Estimates the number of node index entries in the given stream.
        public static int GetNodeIndexEntryCount(Stream stream) => (int) stream.Length / SIZE_OF_NODE_INDEX_ENTRY;

        /// Translates the given node index details into the appropriate byte representation and writes it to this stream at the current position.
        public static void WriteNodeIndexEntry(Stream stream, ulong hash, int dataStart, int dataLength)
        {
            PandoUtils.BitConverter.CopyBytes(hash, nodeIndexBuffer, 0);
            PandoUtils.BitConverter.CopyBytes(dataStart, nodeIndexBuffer, sizeof(ulong));
            PandoUtils.BitConverter.CopyBytes(dataLength, nodeIndexBuffer, sizeof(ulong) + sizeof(int));
            stream.Write(nodeIndexBuffer, 0, SIZE_OF_NODE_INDEX_ENTRY);
        }

        /// Parses a node index hash and slice from this stream of bytes, starting from the current position.
        public static bool ReadNextNodeIndexEntry(Stream stream, out ulong hash, out DataSlice slice)
        {
            var bytesRead = stream.Read(nodeIndexBuffer, 0, SIZE_OF_NODE_INDEX_ENTRY);
            switch (bytesRead)
            {
                case 0:
                    hash = default;
                    slice = default;
                    return false;
                case < SIZE_OF_NODE_INDEX_ENTRY:
                    throw new IncompleteReadException("Could not read a full node index from the remaining bytes in the stream");
            }

            hash = PandoUtils.BitConverter.ToUInt64(nodeIndexBuffer, 0);
            var start = PandoUtils.BitConverter.ToInt32(nodeIndexBuffer, sizeof(ulong));
            var length = PandoUtils.BitConverter.ToInt32(nodeIndexBuffer, sizeof(ulong) + sizeof(int));
            slice = new DataSlice(start, length);
            return true;
        }

        private const int SIZE_OF_SNAPSHOT_INDEX_ENTRY = sizeof(ulong) + sizeof(ulong) + sizeof(ulong);
        private static readonly byte[] writeSnapshotIndexBuffer = new byte[SIZE_OF_SNAPSHOT_INDEX_ENTRY];

        /// Estimates the number of node index entries in the given stream.
        public static int GetSnapshotIndexEntryCount(Stream stream) => (int) stream.Length / SIZE_OF_SNAPSHOT_INDEX_ENTRY;

        /// Translates the given snapshot index details into the appropriate byte representation and writes it to this stream at the current position.
        public static void WriteSnapshotIndexEntry(Stream stream, ulong hash, ulong parentHash, ulong rootNodeHash)
        {
            PandoUtils.BitConverter.CopyBytes(hash, writeSnapshotIndexBuffer, 0);
            PandoUtils.BitConverter.CopyBytes(parentHash, writeSnapshotIndexBuffer, sizeof(ulong));
            PandoUtils.BitConverter.CopyBytes(rootNodeHash, writeSnapshotIndexBuffer, sizeof(ulong) + sizeof(ulong));
            stream.Write(writeSnapshotIndexBuffer, 0, SIZE_OF_SNAPSHOT_INDEX_ENTRY);
        }

        /// Parses a snapshot hash and data from this stream of bytes, starting from the current position.
        public static bool ReadNextSnapshotIndexEntry(Stream stream, out ulong hash, out SnapshotData data)
        {
            var bytesRead = stream.Read(writeSnapshotIndexBuffer, 0, SIZE_OF_SNAPSHOT_INDEX_ENTRY);
            switch (bytesRead)
            {
                case 0:
                    hash = default;
                    data = default;
                    return false;
                case < SIZE_OF_SNAPSHOT_INDEX_ENTRY:
                    throw new IncompleteReadException("Could not read a full snapshot index from the remaining bytes in the stream");
            }

            hash = PandoUtils.BitConverter.ToUInt64(writeSnapshotIndexBuffer, 0);
            var parentHash = PandoUtils.BitConverter.ToUInt64(writeSnapshotIndexBuffer, sizeof(ulong));
            var rootNodeHash = PandoUtils.BitConverter.ToUInt64(writeSnapshotIndexBuffer, sizeof(ulong) + sizeof(ulong));
            data = new SnapshotData(parentHash, rootNodeHash);
            return true;
        }
    }

    internal class IncompleteReadException : Exception
    {
        public IncompleteReadException(string message) : base(message) { }
    }
}
