using System;
using System.Collections.Generic;
using FluentAssertions;
using Pando;
using Pando.Exceptions;
using Pando.Repositories;
using Pando.Repositories.Utils;
using PandoTests.Tests.PandoSave.TestStateTrees;
using Xunit;

namespace PandoTests.Tests.PandoSave;

public class PandoSaveTests
{
	private static TestTree MakeTestTree1()
	{
		const string NAME = "My Name";
		const int AGE = 15;
		var date = new DateTime(2000, 06, 23);
		const int CENTS = 2000;
		return new TestTree(NAME, new TestTree.A(AGE), new TestTree.B(date, CENTS));
	}

	private static TestTree MakeTestTree2()
	{
		const string NAME = "Some Other name";
		const int AGE = 200;
		var date = new DateTime(1971, 12, 30);
		const int CENTS = 1500;
		return new TestTree(NAME, new TestTree.A(AGE), new TestTree.B(date, CENTS));
	}

	public class SaveSnapshot
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
			var snapshotHash = saver.SaveSnapshot(tree);
			TestTree actual = saver.GetSnapshot(snapshotHash);

			// Assert
			actual.Should()
				.NotBeSameAs(tree)
				.And.BeEquivalentTo(tree);
		}

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
			var snapshotHash1 = saver.SaveSnapshot(tree1);
			var snapshotHash2 = saver.SaveSnapshot(tree1Copy);

			// Assert
			snapshotHash1.Should().NotBe(snapshotHash2);
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
			saver.SaveSnapshot(tree);
			var dataArraySnapshot1 = nodeData.VisitSpan(0, nodeData.Count, bytes => bytes.ToArray());

			saver.SaveSnapshot(tree2);
			var dataArraySnapshot2 = nodeData.VisitSpan(0, nodeData.Count, bytes => bytes.ToArray());

			// Assert
			nodeIndex.Count.Should().Be(4, "because a TestTree has 4 blobs");
			dataArraySnapshot1.Length.Should().Be(dataArraySnapshot2.Length);
		}
	}

	public class GetFullSnapshotChain
	{
		[Fact]
		public void Should_return_correct_chain_structure()
		{
			// Test Data
			var tree1 = MakeTestTree1();
			var tree2 = MakeTestTree2();

			// Arrange
			var saver = new PandoSaver<TestTree>(
				new InMemoryRepository(),
				TestTreeSerializer.Create()
			);
			saver.SaveSnapshot(tree1);
			saver.SaveSnapshot(tree2);

			// Act
			var snapshotChain = saver.GetFullSnapshotChain();

			// Assert
			var snapshot1 = snapshotChain;
			var snapshot2 = snapshot1.Children[0];
			snapshot1.Children.Length.Should().Be(1);
			snapshot2.Children.Length.Should().Be(0);
		}

		[Fact]
		public void Should_return_correct_tree()
		{
			// Test Data
			var tree1 = MakeTestTree1();
			var tree2 = MakeTestTree2();

			// Arrange
			var saver = new PandoSaver<TestTree>(
				new InMemoryRepository(),
				TestTreeSerializer.Create()
			);
			saver.SaveSnapshot(tree1);
			saver.SaveSnapshot(tree2);

			// Act
			var snapshotChain = saver.GetFullSnapshotChain();

			// Assert
			var snapshot1 = snapshotChain;
			var snapshot2 = snapshot1.Children[0];
			snapshot1.GetTreeRoot().Should().BeEquivalentTo(tree1);
			snapshot2.GetTreeRoot().Should().BeEquivalentTo(tree2);
		}

		[Fact]
		public void Should_throw_when_no_root_snapshot_exists()
		{
			var saver = new PandoSaver<TestTree>(
				new InMemoryRepository(),
				TestTreeSerializer.Create()
			);

			saver.Invoking(innerSaver => innerSaver.GetFullSnapshotChain())
				.Should()
				.Throw<NoRootSnapshotException>();
		}
	}
}
