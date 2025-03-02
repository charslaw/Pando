using System.Linq;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Repositories;

namespace PandoTests.Tests.DataSources.MemorySnapshotStoreTests;

public static partial class MemorySnapshotStoreTests
{
	public class GetSnapshotChildren
	{
		[Test]
		public async Task Should_return_empty_enumerable_when_no_children_have_been_added()
		{
			var snapshotTree = new MemorySnapshotStore();

			var rootId = snapshotTree.AddRootSnapshot(NodeId.None);

			var children = snapshotTree.GetSnapshotChildren(rootId).ToArray();

			await Assert.That(children).IsEmpty();
		}

		[Test]
		public async Task Should_enumerate_added_children()
		{
			var snapshotTree = new MemorySnapshotStore();

			var rootSnapshotId = snapshotTree.AddRootSnapshot(NodeId.None);
			snapshotTree.AddSnapshot(new NodeId(2), rootSnapshotId);
			snapshotTree.AddSnapshot(new NodeId(3), rootSnapshotId);
			snapshotTree.AddSnapshot(new NodeId(4), rootSnapshotId);

			var children = snapshotTree.GetSnapshotChildren(rootSnapshotId).ToArray();

			SnapshotId[] expected =
			[
				HashUtils.ComputeSnapshotHash(new NodeId(2), rootSnapshotId),
				HashUtils.ComputeSnapshotHash(new NodeId(3), rootSnapshotId),
				HashUtils.ComputeSnapshotHash(new NodeId(4), rootSnapshotId),
			];
			await Assert.That(children).IsEquivalentTo(expected);
		}
	}
}
