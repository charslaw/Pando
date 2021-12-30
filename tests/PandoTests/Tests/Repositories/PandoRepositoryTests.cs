using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Exceptions;
using Pando.Repositories;
using PandoTests.Tests.Repositories.TestStateTrees;
using Xunit;

namespace PandoTests.Tests.Repositories;

public class PandoRepositoryTests
{
	private static TestTree MakeTestTree1() =>
		new(
			"Test Tree 1",
			new TestTree.A(1),
			new TestTree.B(
				new DateTime(1970, 01, 01),
				1000
			)
		);

	private static TestTree MakeTestTree2() =>
		new(
			"Test Tree 2",
			new TestTree.A(2),
			new TestTree.B(
				new DateTime(1970, 02, 02),
				2000
			)
		);

	private static TestTree MakeTestTree3() =>
		new(
			"Test Tree 3",
			new TestTree.A(3),
			new TestTree.B(
				new DateTime(1970, 03, 03),
				3000
			)
		);

	private static TestTree MakeTestTree4() =>
		new(
			"Test Tree 4",
			new TestTree.A(4),
			new TestTree.B(
				new DateTime(1970, 04, 04),
				4000
			)
		);

	private static TestTree MakeTestTree5() =>
		new(
			"Test Tree 5",
			new TestTree.A(5),
			new TestTree.B(
				new DateTime(1970, 05, 05),
				5000
			)
		);

	public class SaveRootSnapshot
	{
		[Fact]
		public void Should_save_state_tree()
		{
			// Test Data
			var tree = MakeTestTree1();

			// Arrange
			var repository = new PandoRepository<TestTree>(
				new MemoryDataSource(),
				TestTreeSerializer.Create()
			);

			// Act
			var snapshotHash = repository.SaveRootSnapshot(tree);
			TestTree actual = repository.GetSnapshot(snapshotHash);

			// Assert
			actual.Should()
				.NotBeSameAs(tree)
				.And.BeEquivalentTo(tree);
		}

		[Fact]
		public void Should_not_create_duplicate_blobs_when_equivalent_state_tree_is_added()
		{
			// Test Data
			var tree = MakeTestTree1();
			var tree2 = tree with { };

			// Arrange
			var nodeIndex = new Dictionary<ulong, DataSlice>();
			var nodeData = new SpannableList<byte>();
			var repository = new PandoRepository<TestTree>(
				new MemoryDataSource(nodeIndex: nodeIndex, nodeData: nodeData),
				TestTreeSerializer.Create()
			);

			// Act
			var rootHash = repository.SaveRootSnapshot(tree);
			repository.SaveSnapshot(tree2, rootHash);

			// Assert
			nodeIndex.Count.Should().Be(4, "because a TestTree has 4 blobs");
		}

		[Fact]
		public void Should_throw_if_a_root_snapshot_already_exists()
		{
			// Test Data
			var tree = MakeTestTree1();
			var tree2 = MakeTestTree2();

			// Arrange
			var repository = new PandoRepository<TestTree>(
				new MemoryDataSource(),
				TestTreeSerializer.Create()
			);
			repository.SaveRootSnapshot(tree);

			// Assert
			repository.Invoking(s => s.SaveRootSnapshot(tree2))
				.Should()
				.Throw<AlreadyHasRootSnapshotException>();
		}
	}

	public class SaveSnapshot
	{
		[Fact]
		public void Should_create_a_new_snapshot_even_if_state_tree_is_equivalent()
		{
			// Test Data
			var tree1 = MakeTestTree1();
			var tree1Copy = tree1 with { };

			// Arrange
			var repository = new PandoRepository<TestTree>(
				new MemoryDataSource(),
				TestTreeSerializer.Create()
			);

			// Act
			var snapshotHash1 = repository.SaveRootSnapshot(tree1);
			var snapshotHash2 = repository.SaveSnapshot(tree1Copy, snapshotHash1);

			// Assert
			snapshotHash1.Should().NotBe(snapshotHash2);
		}
	}

	public class GetSnapshotTree
	{
		[Fact]
		public void Should_return_correct_tree()
		{
			// Test Data
			var tree1 = MakeTestTree1();
			var tree2 = MakeTestTree2();
			var tree3 = MakeTestTree3();
			var tree4 = MakeTestTree4();

			// Arrange
			var repository = new PandoRepository<TestTree>(
				new MemoryDataSource(),
				TestTreeSerializer.Create()
			);
			var rootHash = repository.SaveRootSnapshot(tree1);
			var child1Hash = repository.SaveSnapshot(tree2, rootHash);
			var child2Hash = repository.SaveSnapshot(tree3, rootHash);
			var grandChildHash = repository.SaveSnapshot(tree4, child1Hash);

			// Act
			SnapshotTree snapshotTree = repository.GetSnapshotTree();

			// Assert
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
				TestTreeSerializer.Create()
			);

			// Act
			SnapshotTree snapshotTree = repository.GetSnapshotTree();

			// Assert
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

			var repository = new PandoRepository<TestTree>(
				source,
				TestTreeSerializer.Create()
			);

			// Act
			var newHash = repository.SaveSnapshot(MakeTestTree5(), child2Hash);

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
				TestTreeSerializer.Create()
			);

			repository.Invoking(s => s.GetSnapshotTree()).Should().Throw<NoRootSnapshotException>();
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
				TestTreeSerializer.Create()
			);

			var rootHash = repository.SaveRootSnapshot(MakeTestTree1());
			var child1Hash = repository.SaveSnapshot(MakeTestTree2(), rootHash);
			var child2Hash = repository.SaveSnapshot(MakeTestTree3(), rootHash);
			var grandChildHash = repository.SaveSnapshot(MakeTestTree4(), child1Hash);

			// Reconstitute a new data source to return
			var source = new MemoryDataSource(snapshotIndex, leafSnapshots, nodeIndex, nodeData);

			return (source, rootHash, child1Hash, child2Hash, grandChildHash);
		}
	}
}
