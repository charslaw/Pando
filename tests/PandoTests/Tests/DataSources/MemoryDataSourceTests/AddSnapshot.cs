using System.Collections.Generic;
using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Xunit;

namespace PandoTests.Tests.DataSources.MemoryDataSourceTests;

public class AddSnapshot
{
	[Fact]
	public void Should_add_to_snapshot_index()
	{
		// Test Data
		var parentSnapshotId = new SnapshotId(1);
		var rootNodeId = new NodeId(2);

		// Arrange
		var snapshotIndex = new Dictionary<SnapshotId, SnapshotData>();
		var dataSource = new MemoryDataSource(snapshotIndex: snapshotIndex);

		// Act
		dataSource.AddSnapshot(parentSnapshotId, rootNodeId);

		// Assert
		snapshotIndex.Count.Should().Be(1);
	}

	[Fact]
	public void Should_not_throw_on_duplicate_snapshot()
	{
		// Test Data
		var parentSnapshotId = new SnapshotId(1);
		var rootNodeId = new NodeId(2);

		// Arrange
		var dataSource = new MemoryDataSource();

		// Assert
		dataSource.Invoking(source =>
				{
					source.AddSnapshot(parentSnapshotId, rootNodeId);
					source.AddSnapshot(parentSnapshotId, rootNodeId);
				}
			)
			.Should()
			.NotThrow();
	}
}
