using System.Collections.Generic;
using Pando.Repositories;
using Pando.Vaults;

namespace PandoTests.Tests.Vaults.MemorySnapshotVaultTests;

public static partial class MemorySnapshotVaultTests
{
	public class WalkTree
	{
		[Test]
		public async Task Should_enumerate_descendants_depth_first_in_order_of_insertion()
		{
			var snapshotTree = new MemorySnapshotVault();

			var root = snapshotTree.AddRootSnapshot(new NodeId(1));
			var s2 = snapshotTree.AddSnapshot(new NodeId(2), root);
			var s3 = snapshotTree.AddSnapshot(new NodeId(3), root);
			var s4 = snapshotTree.AddSnapshot(new NodeId(4), s2);
			var s5 = snapshotTree.AddSnapshot(new NodeId(5), s4);
			_ = snapshotTree.AddSnapshot(new NodeId(6), s3);
			_ = snapshotTree.AddSnapshot(new NodeId(7), s5);
			_ = snapshotTree.AddSnapshot(new NodeId(8), s5);

			List<ulong> enumerationOrder = [];
			snapshotTree.WalkTree((_, _, _, nodeId) => enumerationOrder.Add(nodeId.Hash));

			List<ulong> expected = [1, 2, 4, 5, 7, 8, 3, 6];
			await Assert.That(enumerationOrder).IsEquivalentTo(expected);
		}
	}
}
