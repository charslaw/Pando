using System.Collections.Generic;
using Pando.DataSources;
using Pando.Repositories;

namespace PandoTests.Tests.DataSources.MemorySnapshotStoreTests;

public static partial class MemorySnapshotStoreTests
{
	public class AddSnapshot
	{
		[Test]
		public async Task Should_add_to_snapshot_index()
		{
			var snapshotIndex = new Dictionary<SnapshotId, MemorySnapshotStore.TreeEntry>();
			var dataSource = new MemorySnapshotStore(snapshotIndex: snapshotIndex);

			var rootSnapshotId = dataSource.AddRootSnapshot(NodeId.None);
			dataSource.AddSnapshot(new NodeId(1), rootSnapshotId);

			await Assert.That(snapshotIndex).HasCount().EqualTo(2);
		}

		[Test]
		public async Task Should_not_throw_on_duplicate_snapshot()
		{
			var dataSource = new MemorySnapshotStore();

			var rootSnapshotId = dataSource.AddRootSnapshot(NodeId.None);

			await Assert
				.That(() =>
				{
					dataSource.AddSnapshot(new NodeId(1), rootSnapshotId);
					dataSource.AddSnapshot(new NodeId(1), rootSnapshotId);
				})
				.ThrowsNothing();
		}

		[Test]
		public async Task Should_not_add_duplicate_snapshot()
		{
			var snapshotIndex = new Dictionary<SnapshotId, MemorySnapshotStore.TreeEntry>();
			var dataSource = new MemorySnapshotStore(snapshotIndex: snapshotIndex);

			var rootSnapshotId = dataSource.AddRootSnapshot(NodeId.None);
			dataSource.AddSnapshot(new NodeId(1), rootSnapshotId);
			dataSource.AddSnapshot(new NodeId(1), rootSnapshotId);

			await Assert.That(snapshotIndex).HasCount().EqualTo(2);
		}
	}
}
