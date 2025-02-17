using System;
using FluentAssertions;
using Pando.DataSources;
using Pando.Exceptions;
using Pando.Repositories;
using PandoTests.Tests.Repositories.TestStateTrees;
using Xunit;

namespace PandoTests.Tests.Repositories.PandoRepositoryTests;

public class SaveRootSnapshot
{
	[Fact]
	public void Should_save_state_tree()
	{
		// Test Data
		var tree = new TestTree(
			"Test Tree 1",
			new TestTree.A(1),
			new TestTree.B(
				new DateTime(1970, 01, 01),
				1000
			)
		);

		// Arrange
		var repository = new PandoRepository<TestTree>(
			new MemoryDataSource(),
			TestTree.GenericSerializer()
		);

		// Act
		var snapshotHash = repository.SaveRootSnapshot(tree);
		TestTree actual = repository.GetSnapshot(snapshotHash);

		// Assert
		actual.Should()
			.NotBeSameAs(tree)
			.And.BeEquivalentTo(tree, options => options.ComparingByMembers<TestTree>());
	}



	[Fact]
	public void Should_throw_if_a_root_snapshot_already_exists()
	{
		// Test Data
		var tree = new TestTree(
			"Test Tree 1",
			new TestTree.A(1),
			new TestTree.B(
				new DateTime(1970, 01, 01),
				1000
			)
		);
		var tree2 = tree with { Name = "Different Tree" };

		// Arrange
		var repository = new PandoRepository<TestTree>(
			new MemoryDataSource(),
			TestTree.GenericSerializer()
		);
		repository.SaveRootSnapshot(tree);

		// Assert
		repository.Invoking(s => s.SaveRootSnapshot(tree2))
			.Should()
			.Throw<AlreadyHasRootSnapshotException>();
	}
}
