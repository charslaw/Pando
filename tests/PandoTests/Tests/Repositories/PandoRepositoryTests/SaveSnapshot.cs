using System;
using System.Collections.Generic;
using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Exceptions;
using Pando.Repositories;
using PandoTests.Tests.Repositories.TestStateTrees;
using Xunit;

namespace PandoTests.Tests.Repositories.PandoRepositoryTests;

public class SaveSnapshot
{
	[Fact]
	public void Should_create_a_new_snapshot_even_if_state_tree_is_equivalent()
	{
		var tree1 = new TestTree(
			"Test Tree 1",
			new TestTree.A(1),
			new TestTree.B(
				new DateTime(1970, 01, 01),
				1000
			)
		);
		var tree1Copy = tree1 with { };

		var repository = new PandoRepository<TestTree>(
			new MemoryDataSource(),
			TestTree.GenericSerializer()
		);

		var snapshotHash1 = repository.SaveRootSnapshot(tree1);
		var snapshotHash2 = repository.SaveSnapshot(tree1Copy, snapshotHash1);

		snapshotHash1.Should().NotBe(snapshotHash2);
	}

	[Fact]
	public void Should_not_create_duplicate_blobs_when_equivalent_state_tree_is_added()
	{
		var tree = new TestTree(
			"Test Tree 1",
			new TestTree.A(1),
			new TestTree.B(
				new DateTime(1970, 01, 01),
				1000
			)
		);
		var tree2 = tree with { };

		var nodeIndex = new Dictionary<ulong, DataSlice>();
		var nodeData = new SpannableList<byte>();
		var repository = new PandoRepository<TestTree>(
			new MemoryDataSource(nodeIndex: nodeIndex, nodeData: nodeData),
			TestTree.GenericSerializer()
		);

		var rootHash = repository.SaveRootSnapshot(tree);
		repository.SaveSnapshot(tree2, rootHash);

		nodeIndex.Count.Should().Be(4, "because a TestTree is made of 3 nodes, plus the root node");
	}

	[Fact]
	public void Should_throw_if_given_parent_snapshot_hash_doesnt_exist()
	{
		var tree1 = new TestTree(
			"Test Tree 1",
			new TestTree.A(1),
			new TestTree.B(
				new DateTime(1970, 01, 01),
				1000
			)
		);

		var source = new MemoryDataSource();
		var repository = new PandoRepository<TestTree>(
			source,
			TestTree.GenericSerializer()
		);

		repository.Invoking(repo => repo.SaveSnapshot(tree1, 0UL))
			.Should()
			.Throw<HashNotFoundException>();

		source.SnapshotCount.Should().Be(0, "because it should check the validity prior to making any changes to the data source");
	}
}
