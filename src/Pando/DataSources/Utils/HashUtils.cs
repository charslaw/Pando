using System;
using Pando.Repositories;
using Standart.Hash.xxHash;

namespace Pando.DataSources.Utils;

internal static class HashUtils
{
	public static NodeId ComputeNodeHash(ReadOnlySpan<byte> nodeData) => new(xxHash64.ComputeHash(nodeData, nodeData.Length));

	public static SnapshotId ComputeSnapshotHash(NodeId rootNodeId, SnapshotId sourceSnapshotId, SnapshotId targetSnapshotId)
	{
		Span<byte> buffer = stackalloc byte[SnapshotId.SIZE + SnapshotId.SIZE + NodeId.SIZE];
		sourceSnapshotId.CopyTo(buffer.Slice(0, SnapshotId.SIZE));
		targetSnapshotId.CopyTo(buffer.Slice(SnapshotId.SIZE, SnapshotId.SIZE));
		rootNodeId.CopyTo(buffer.Slice(SnapshotId.SIZE * 2, NodeId.SIZE));

		var snapshotHash = xxHash64.ComputeHash(buffer, SnapshotId.SIZE + SnapshotId.SIZE + NodeId.SIZE);
		return new SnapshotId(snapshotHash);
	}

	public static SnapshotId ComputeSnapshotHash(NodeId rootNodeId, SnapshotId sourceSnapshotId) =>
		ComputeSnapshotHash(rootNodeId, sourceSnapshotId, SnapshotId.None);
}
