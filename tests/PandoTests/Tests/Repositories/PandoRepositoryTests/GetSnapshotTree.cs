using System;
using System.Collections.Immutable;
using System.IO;
using FluentAssertions;
using Pando.DataSources;
using Pando.Exceptions;
using Pando.Repositories;
using PandoTests.Tests.Repositories.TestStateTrees;
using Xunit;

namespace PandoTests.Tests.Repositories.PandoRepositoryTests;

public class GetSnapshotTree
{
	[Fact]
	public void Should_return_correct_tree()
	{
		var tree1 = new TestTree(
			"Test Tree 1",
			new TestTree.A(1),
			new TestTree.B(
				new DateTime(1970, 01, 01),
				1000
			)
		);
		var tree2 = tree1 with { Name = "Different Tree" };
		var tree3 = tree2 with { MyA = new TestTree.A(2) };
		var tree4 = tree3 with { MyB = new TestTree.B(tree3.MyB.Time.AddDays(1), 2000) };

		var repository = new PandoRepository<TestTree>(
			new MemoryDataSource(),
			TestTree.GenericSerializer()
		);
		var rootHash = repository.SaveRootSnapshot(tree1);
		var child1Hash = repository.SaveSnapshot(tree2, rootHash);
		var child2Hash = repository.SaveSnapshot(tree3, rootHash);
		var grandChildHash = repository.SaveSnapshot(tree4, child1Hash);

		SnapshotTree snapshotTree = repository.GetSnapshotTree();

		var expected = new SnapshotTree(
			rootHash,
			ImmutableArray.Create(
				new SnapshotTree(
					child1Hash,
					ImmutableArray.Create(
						new SnapshotTree(grandChildHash)
					)
				),
				new SnapshotTree(child2Hash)
			)
		);

		snapshotTree.Should().BeEquivalentTo(expected, options => options.ComparingByMembers<SnapshotTree>());
	}

	[Fact]
	public void Should_return_correct_tree_when_reconstituting_from_persisted_data()
	{
		var (source, rootHash, child1Hash, child2Hash, grandChildHash) = PrepopulatedDataSource();

		var repository = new PandoRepository<TestTree>(
			source,
			TestTree.GenericSerializer()
		);

		SnapshotTree snapshotTree = repository.GetSnapshotTree();

		var expected = new SnapshotTree(
			rootHash,
			ImmutableArray.Create(
				new SnapshotTree(
					child1Hash,
					ImmutableArray.Create(
						new SnapshotTree(grandChildHash)
					)
				),
				new SnapshotTree(child2Hash)
			)
		);

		snapshotTree.Should().BeEquivalentTo(expected, options => options.ComparingByMembers<SnapshotTree>());
	}

	[Fact]
	public void Should_return_correct_tree_when_adding_to_reconstituted_repository()
	{
		var (source, rootHash, child1Hash, child2Hash, grandChildHash) = PrepopulatedDataSource();

		var newTree = new TestTree(
			"New Test Tree",
			new TestTree.A(42),
			new TestTree.B(
				new DateTime(2000, 05, 04),
				1234
			)
		);

		var repository = new PandoRepository<TestTree>(
			source,
			TestTree.GenericSerializer()
		);

		// Act
		var newHash = repository.SaveSnapshot(newTree, child2Hash);

		// Assert
		SnapshotTree snapshotTree = repository.GetSnapshotTree();
		var expected = new SnapshotTree(
			rootHash,
			ImmutableArray.Create(
				new SnapshotTree(
					child1Hash,
					ImmutableArray.Create(
						new SnapshotTree(grandChildHash)
					)
				),
				new SnapshotTree(
					child2Hash,
					ImmutableArray.Create(
						new SnapshotTree(newHash)
					)
				)
			)
		);

		snapshotTree.Should().BeEquivalentTo(expected, options => options.ComparingByMembers<SnapshotTree>());
	}

	[Fact]
	public void Should_throw_if_no_root_snapshot()
	{
		var repository = new PandoRepository<TestTree>(
			new MemoryDataSource(),
			TestTree.GenericSerializer()
		);

		repository.Invoking(repo => repo.GetSnapshotTree()).Should().Throw<NoRootSnapshotException>();
	}

	/// Simulate adding snapshots to the data source in a previous session, then reconstitute a new data source from the persisted data
	/// This is used to test that a Pando repository will work properly when used with a data source that already has data.
	private static (IDataSource source, ulong rootHash, ulong child1Hash, ulong child2Hash, ulong grandChildHash) PrepopulatedDataSource()
	{
		// Assemble "previous session" data
		var snapshotIndex = new MemoryStream();
		var leafSnapshots = new MemoryStream();
		var nodeIndex = new MemoryStream();
		var nodeData = new MemoryStream();

		var repository = new PandoRepository<TestTree>(
			new PersistenceBackedDataSource(
				new MemoryDataSource(),
				new StreamDataSource(snapshotIndex, leafSnapshots, nodeIndex, nodeData)
			),
			TestTree.GenericSerializer()
		);

		var tree1 = new TestTree(
			"Test Tree 1",
			new TestTree.A(1),
			new TestTree.B(
				new DateTime(1970, 01, 01),
				1000
			)
		);
		var tree2 = tree1 with { Name = "Different Tree" };
		var tree3 = tree2 with { MyA = new TestTree.A(2) };
		var tree4 = tree3 with { MyB = new TestTree.B(tree3.MyB.Time.AddDays(1), 2000) };

		var rootHash = repository.SaveRootSnapshot(tree1);
		var child1Hash = repository.SaveSnapshot(tree2, rootHash);
		var child2Hash = repository.SaveSnapshot(tree3, rootHash);
		var grandChildHash = repository.SaveSnapshot(tree4, child1Hash);

		// Reconstitute a new data source to return
		var source = new MemoryDataSource(snapshotIndex, leafSnapshots, nodeIndex, nodeData);

		return (source, rootHash, child1Hash, child2Hash, grandChildHash);
	}
}
