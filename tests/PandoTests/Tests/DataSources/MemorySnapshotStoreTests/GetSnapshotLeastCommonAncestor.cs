using FluentAssertions;
using Pando.DataSources;
using Pando.Repositories;
using Xunit;

namespace PandoTests.Tests.DataSources.MemorySnapshotStoreTests;

public class GetSnapshotLeastCommonAncestor
{
	[Fact]
	public void Should_return_correct_least_common_ancestor_snapshot_hash()
	{
		var dataSource = new MemorySnapshotStore();

		var rootId = dataSource.AddRootSnapshot(new NodeId(1));
		var childId = dataSource.AddSnapshot(new NodeId(2), rootId);
		var branch1Id = dataSource.AddSnapshot(new NodeId(3), childId);
		var branch2AId = dataSource.AddSnapshot(new NodeId(4), childId);
		var branch2BId = dataSource.AddSnapshot(new NodeId(5), branch2AId);

		var lca = dataSource.GetSnapshotLeastCommonAncestor(branch1Id, branch2BId);

		lca.Should().Be(childId);
	}

	[Fact]
	public void Should_return_correct_lca_when_first_snapshot_is_ancestor_of_second()
	{
		var dataSource = new MemorySnapshotStore();
		var rootId = dataSource.AddRootSnapshot(new NodeId(1));
		var childId = dataSource.AddSnapshot(new NodeId(2), rootId);
		var grandchildId = dataSource.AddSnapshot(new NodeId(3), childId);

		var lca = dataSource.GetSnapshotLeastCommonAncestor(childId, grandchildId);


		lca.Should().Be(childId);
	}



	[Fact]
	public void Should_return_correct_lca_when_second_snapshot_is_ancestor_of_first()
	{
		var dataSource = new MemorySnapshotStore();
		var rootId = dataSource.AddRootSnapshot(new NodeId(1));
		var childId = dataSource.AddSnapshot(new NodeId(2), rootId);
		var grandchildId = dataSource.AddSnapshot(new NodeId(3), childId);

		var lca = dataSource.GetSnapshotLeastCommonAncestor(grandchildId, childId);

		lca.Should().Be(childId);
	}
}

