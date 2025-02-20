using System;
using Standart.Hash.xxHash;

namespace Pando.DataSources.Utils;

internal static class HashUtils
{
	public static NodeId ComputeNodeHash(ReadOnlySpan<byte> nodeData) => new(xxHash64.ComputeHash(nodeData, nodeData.Length));

	private const int END_OF_PARENT_HASH = SnapshotId.SIZE;
	private const int END_OF_ROOT_NODE_HASH = END_OF_PARENT_HASH + NodeId.SIZE;
	private const int SIZE_OF_SNAPSHOT_BUFFER = END_OF_ROOT_NODE_HASH;

	public static SnapshotId ComputeSnapshotHash(SnapshotId parentId, NodeId rootNodeId)
	{
		Span<byte> buffer = stackalloc byte[SIZE_OF_SNAPSHOT_BUFFER];
		parentId.CopyTo(buffer[..END_OF_PARENT_HASH]);
		rootNodeId.CopyTo(buffer[END_OF_PARENT_HASH..END_OF_ROOT_NODE_HASH]);
		return new SnapshotId(xxHash64.ComputeHash(buffer, SIZE_OF_SNAPSHOT_BUFFER));
	}
}
