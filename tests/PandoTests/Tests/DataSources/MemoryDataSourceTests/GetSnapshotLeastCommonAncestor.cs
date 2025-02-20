using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Xunit;

namespace PandoTests.Tests.DataSources.MemoryDataSourceTests;

public class GetSnapshotLeastCommonAncestor
{
	[Fact]
	public void Should_return_correct_least_common_ancestor_snapshot_hash()
	{
		var dataSource = new MemoryDataSource();

		var rootId = dataSource.AddSnapshot(new SnapshotId(0), new NodeId(1));
		var childId = dataSource.AddSnapshot(rootId, new NodeId(1));
		var branch1Id = dataSource.AddSnapshot(childId, new NodeId(1));
		var branch2AId = dataSource.AddSnapshot(childId, new NodeId(2));
		var branch2BId = dataSource.AddSnapshot(branch2AId, new NodeId(1));

		var lca = dataSource.GetSnapshotLeastCommonAncestor(branch1Id, branch2BId);

		lca.Should().Be(childId);
	}
}

