using System.Linq;
using FluentAssertions;
using Pando.DataSources.Utils;
using Xunit;
using Pando.DataStructures;

namespace PandoTests.Tests.DataStructures.SnapshotTreeTests;

public class GetSnapshotChildren
{
	[Fact]
	public void Should_return_empty_enumerable_when_no_children_have_been_added()
	{
		var snapshotTree = new SnapshotTree();

		var rootSnapshotId = new SnapshotId(1);
		snapshotTree.AddRootSnapshot(rootSnapshotId);

		var children = snapshotTree.GetSnapshotChildren(rootSnapshotId).ToArray();

		children.Should().BeEmpty();
	}

	[Fact]
	public void Should_enumerate_added_children()
	{
		var snapshotTree = new SnapshotTree();

		var rootSnapshotId = new SnapshotId(1);
		snapshotTree.AddRootSnapshot(rootSnapshotId);
		snapshotTree.AddSnapshot(new SnapshotId(2), rootSnapshotId);
		snapshotTree.AddSnapshot(new SnapshotId(3), rootSnapshotId);
		snapshotTree.AddSnapshot(new SnapshotId(4), rootSnapshotId);

		var children = snapshotTree.GetSnapshotChildren(rootSnapshotId).ToArray();

		children.Should().BeEquivalentTo([new SnapshotId(2), new SnapshotId(3), new SnapshotId(4)]);
	}
}
