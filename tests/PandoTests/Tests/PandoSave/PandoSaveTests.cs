using System;
using System.Collections.Generic;
using FluentAssertions;
using Pando;
using Pando.Exceptions;
using Pando.Repositories;
using Pando.Repositories.Utils;
using PandoTests.Tests.PandoSave.TestStateTrees;
using PandoTests.Utils;
using Xunit;

namespace PandoTests.Tests.PandoSave;

public class PandoSaveTests
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

	public class SaveRootSnapshot
	{
		[Fact]
		public void Should_save_state_tree()
		{
			// Test Data
			var tree = MakeTestTree1();

			// Arrange
			var saver = new PandoSaver<TestTree>(
				new InMemoryRepository(),
				TestTreeSerializer.Create()
			);

			// Act
			var snapshotHash = saver.SaveRootSnapshot(tree);
			TestTree actual = saver.GetSnapshot(snapshotHash);

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
			var saver = new PandoSaver<TestTree>(
				new InMemoryRepository(nodeIndex: nodeIndex, nodeData: nodeData),
				TestTreeSerializer.Create()
			);

			// Act
			var rootHash = saver.SaveRootSnapshot(tree);
			var dataArraySnapshot1 = nodeData.VisitSpan(0, nodeData.Count, new ToArrayVisitor());

			saver.SaveSnapshot(tree2, rootHash);
			var dataArraySnapshot2 = nodeData.VisitSpan(0, nodeData.Count, new ToArrayVisitor());

			// Assert
			nodeIndex.Count.Should().Be(4, "because a TestTree has 4 blobs");
			dataArraySnapshot1.Length.Should().Be(dataArraySnapshot2.Length);
		}

		[Fact]
		public void Should_throw_if_a_root_snapshot_already_exists()
		{
			// Test Data
			var tree = MakeTestTree1();
			var tree2 = MakeTestTree2();

			// Arrange
			var saver = new PandoSaver<TestTree>(
				new InMemoryRepository(),
				TestTreeSerializer.Create()
			);
			saver.SaveRootSnapshot(tree);

			// Assert
			saver.Invoking(s => s.SaveRootSnapshot(tree2))
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
			var saver = new PandoSaver<TestTree>(
				new InMemoryRepository(),
				TestTreeSerializer.Create()
			);

			// Act
			var snapshotHash1 = saver.SaveRootSnapshot(tree1);
			var snapshotHash2 = saver.SaveSnapshot(tree1Copy, snapshotHash1);

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
			var saver = new PandoSaver<TestTree>(
				new InMemoryRepository(),
				TestTreeSerializer.Create()
			);
			var rootHash = saver.SaveRootSnapshot(tree1);
			var child1Hash = saver.SaveSnapshot(tree2, rootHash);
			var child2Hash = saver.SaveSnapshot(tree3, rootHash);
			var grandChildHash = saver.SaveSnapshot(tree4, child1Hash);

			// Act
			SnapshotTree snapshotTree = saver.GetSnapshotTree();

			// Assert
			var expected = new
			{
				Hash = rootHash,
				Children = new object[]
				{
					new
					{
						Hash = child1Hash,
						Children = new object[]
						{
							new { Hash = grandChildHash, Children = (object[]?)null },
						}
					},
					new { Hash = child2Hash, Children = (object[]?)null },
				}
			};

			snapshotTree.Should().BeEquivalentTo(expected, options => options.ComparingByMembers<SnapshotTree>());
		}

		[Fact]
		public void Should_throw_if_no_root_snapshot()
		{
			var saver = new PandoSaver<TestTree>(
				new InMemoryRepository(),
				TestTreeSerializer.Create()
			);

			saver.Invoking(s => s.GetSnapshotTree()).Should().Throw<NoRootSnapshotException>();
		}
	}
}
