using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Exceptions;
using Xunit;

namespace PandoTests.Tests.DataSources.MemoryDataSourceTests;

public class GetSnapshotParent
{
	[Fact]
	public void Should_return_correct_parent_snapshot_hash()
	{
		// Test Data
		var parentSnapshotId = new SnapshotId(1);
		var rootNodeId = new NodeId(2);

		// Arrange
		var dataSource = new MemoryDataSource();

		// Act
		var snapshotId = dataSource.AddSnapshot(parentSnapshotId, rootNodeId);

		// Assert
		dataSource.GetSnapshotParent(snapshotId).Should().Be(new SnapshotId(1));
	}

	[Fact]
	public void Should_throw_if_GetSnapshotParent_called_with_nonexistent_hash()
	{
		// Arrange
		var dataSource = new MemoryDataSource();

		// Assert
		dataSource.Invoking(ts => ts.GetSnapshotParent(SnapshotId.None))
			.Should()
			.Throw<SnapshotIdNotFoundException>();
	}
}
