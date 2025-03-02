using Pando.DataSources;
using Pando.Exceptions;
using Pando.Repositories;

namespace PandoTests.Tests.DataSources.MemorySnapshotStoreTests;

public static partial class MemorySnapshotStoreTests
{
	public class GetSnapshotRootNode
	{
		[Test]
		public async Task Should_return_correct_root_node_hash()
		{
			var dataSource = new MemorySnapshotStore();

			var snapshotId = dataSource.AddRootSnapshot(new NodeId(2));

			var nodeId = dataSource.GetSnapshotRootNodeId(snapshotId);
			await Assert.That(nodeId).IsEqualTo(new NodeId(2));
		}

		[Test]
		public async Task Should_throw_if_GetSnapshotRootNode_called_with_nonexistent_hash()
		{
			var dataSource = new MemorySnapshotStore();

			await Assert
				.That(() => dataSource.GetSnapshotRootNodeId(SnapshotId.None))
				.ThrowsExactly<SnapshotIdNotFoundException>();
		}
	}
}
