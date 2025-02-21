using FluentAssertions;
using Pando.DataSources;
using Pando.Exceptions;
using Pando.Repositories;
using Xunit;

namespace PandoTests.Tests.DataSources.MemorySnapshotStoreTests;

public class GetSnapshotRootNode
{
	[Fact]
	public void Should_return_correct_root_node_hash()
	{

		var dataSource = new MemorySnapshotStore();

		var snapshotId = dataSource.AddRootSnapshot(new NodeId(2));

		// Assert
		dataSource.GetSnapshotRootNodeId(snapshotId).Should().Be(new NodeId(2));
	}

	[Fact]
	public void Should_throw_if_GetSnapshotRootNode_called_with_nonexistent_hash()
	{
		// Arrange
		var dataSource = new MemorySnapshotStore();

		// Assert
		dataSource.Invoking(ts => ts.GetSnapshotRootNodeId(SnapshotId.None))
			.Should()
			.Throw<SnapshotIdNotFoundException>();
	}
}
