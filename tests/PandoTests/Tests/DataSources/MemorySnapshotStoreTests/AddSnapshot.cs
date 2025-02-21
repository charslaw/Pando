using System.Collections.Generic;
using FluentAssertions;
using Pando.DataSources;
using Pando.Repositories;
using Xunit;

namespace PandoTests.Tests.DataSources.MemorySnapshotStoreTests;

public class AddSnapshot
{
	[Fact]
	public void Should_add_to_snapshot_index()
	{
		var snapshotIndex = new Dictionary<SnapshotId, MemorySnapshotStore.TreeEntry>();
		var dataSource = new MemorySnapshotStore(snapshotIndex: snapshotIndex);

		var rootSnapshotId = dataSource.AddRootSnapshot(NodeId.None);
		dataSource.AddSnapshot(new NodeId(1), rootSnapshotId);

		snapshotIndex.Count.Should().Be(2);
	}

	[Fact]
	public void Should_not_throw_on_duplicate_snapshot()
	{

		var dataSource = new MemorySnapshotStore();

		var rootSnapshotId = dataSource.AddRootSnapshot(NodeId.None);

		dataSource.Invoking(source =>
				{
					source.AddSnapshot(new NodeId(1), rootSnapshotId);
					source.AddSnapshot(new NodeId(1), rootSnapshotId);
				}
			)
			.Should()
			.NotThrow();
	}

	[Fact]
	public void Should_not_add_duplicate_snapshot()
	{
		var snapshotIndex = new Dictionary<SnapshotId, MemorySnapshotStore.TreeEntry>();
		var dataSource = new MemorySnapshotStore(snapshotIndex: snapshotIndex);

		var rootSnapshotId = dataSource.AddRootSnapshot(NodeId.None);
		dataSource.AddSnapshot(new NodeId(1), rootSnapshotId);
		dataSource.AddSnapshot(new NodeId(1), rootSnapshotId);

		snapshotIndex.Count.Should().Be(2);
	}
}
