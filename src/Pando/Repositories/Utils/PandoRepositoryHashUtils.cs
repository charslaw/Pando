using System;
using Standart.Hash.xxHash;

namespace Pando.Repositories.Utils;

internal static class PandoRepositoryHashUtils
{
	public static ulong ComputeNodeHash(ReadOnlySpan<byte> nodeData) => xxHash64.ComputeHash(nodeData, nodeData.Length);

	private const int SIZE_OF_SNAPSHOT_BUFFER = sizeof(ulong) * 2;

	public static ulong ComputeSnapshotHash(ulong parentHash, ulong rootNodeHash)
	{
		Span<byte> buffer = stackalloc byte[SIZE_OF_SNAPSHOT_BUFFER];
		PandoUtils.BitConverter.CopyBytes(parentHash, buffer.Slice(0, sizeof(ulong)));
		PandoUtils.BitConverter.CopyBytes(rootNodeHash, buffer.Slice(sizeof(ulong), sizeof(ulong)));
		return xxHash64.ComputeHash(buffer, SIZE_OF_SNAPSHOT_BUFFER);
	}
}