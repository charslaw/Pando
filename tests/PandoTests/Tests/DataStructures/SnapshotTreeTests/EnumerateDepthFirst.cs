using System.Linq;
using FluentAssertions;
using Pando.DataSources.Utils;
using Pando.DataStructures;
using Xunit;

namespace PandoTests.Tests.DataStructures.SnapshotTreeTests;

public class EnumerateDepthFirst
{
	[Fact]
	public void Should_enumerate_descendants_depth_first_in_order_of_insertion()
	{
		var snapshotTree = new SnapshotTree();

		snapshotTree.AddRootSnapshot(new SnapshotId(1));
		snapshotTree.AddSnapshot(new SnapshotId(2), new SnapshotId(1));
		snapshotTree.AddSnapshot(new SnapshotId(3), new SnapshotId(1));
		snapshotTree.AddSnapshot(new SnapshotId(4), new SnapshotId(2));
		snapshotTree.AddSnapshot(new SnapshotId(5), new SnapshotId(4));
		snapshotTree.AddSnapshot(new SnapshotId(6), new SnapshotId(3));
		snapshotTree.AddSnapshot(new SnapshotId(7), new SnapshotId(5));
		snapshotTree.AddSnapshot(new SnapshotId(8), new SnapshotId(5));

		var enumerationOrder = snapshotTree.EnumerateDepthFirst().Select(x => x.snapshotId.Hash);

		enumerationOrder.Should().BeEquivalentTo([1, 2, 4, 5, 7, 8, 3, 6]);
	}
}
