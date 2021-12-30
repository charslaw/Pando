using System;
using Standart.Hash.xxHash;

namespace Pando.DataSources.Utils;

internal static class HashUtils
{
	public static ulong ComputeNodeHash(ReadOnlySpan<byte> nodeData) => xxHash64.ComputeHash(nodeData, nodeData.Length);

	private const int END_OF_PARENT_HASH = sizeof(ulong);
	private const int END_OF_ROOT_NODE_HASH = END_OF_PARENT_HASH + sizeof(ulong);
	private const int SIZE_OF_SNAPSHOT_BUFFER = END_OF_ROOT_NODE_HASH;

	public static ulong ComputeSnapshotHash(ulong parentHash, ulong rootNodeHash)
	{
		Span<byte> buffer = stackalloc byte[SIZE_OF_SNAPSHOT_BUFFER];
		ByteEncoder.CopyBytes(parentHash, buffer[..END_OF_PARENT_HASH]);
		ByteEncoder.CopyBytes(rootNodeHash, buffer[END_OF_PARENT_HASH..END_OF_ROOT_NODE_HASH]);
		return xxHash64.ComputeHash(buffer, SIZE_OF_SNAPSHOT_BUFFER);
	}
}
