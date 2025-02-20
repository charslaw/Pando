using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Exceptions;
using Xunit;

namespace PandoTests.Tests.DataSources.MemoryDataSourceTests;

public class GetSnapshotRootNode
{
	[Fact]
	public void Should_return_correct_root_node_hash()
	{
		// Test Data
		var parentSnapshotId = new SnapshotId(1);
		var rootNodeId = new NodeId(2);

		// Arrange
		var dataSource = new MemoryDataSource();

		// Act
		var snapshotId = dataSource.AddSnapshot(parentSnapshotId, rootNodeId);

		// Assert
		dataSource.GetSnapshotRootNode(snapshotId).Should().Be(new NodeId(2));
	}

	[Fact]
	public void Should_throw_if_GetSnapshotRootNode_called_with_nonexistent_hash()
	{
		// Arrange
		var dataSource = new MemoryDataSource();

		// Assert
		dataSource.Invoking(ts => ts.GetSnapshotRootNode(SnapshotId.None))
			.Should()
			.Throw<SnapshotIdNotFoundException>();
	}
}
