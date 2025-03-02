using System;
using Pando.DataSources;
using Pando.Exceptions;
using Pando.Repositories;
using PandoTests.Tests.Repositories.TestStateTrees;

namespace PandoTests.Tests.Repositories.PandoRepositoryTests;

public static partial class PandoRepositoryTests
{
	public class SaveRootSnapshot
	{
		[Test]
		public async Task Should_save_state_tree()
		{
			var tree = new TestTree(
				"Test Tree 1",
				new TestTree.A(1),
				new TestTree.B(
					new DateTime(1970, 01, 01),
					1000
				)
			);

			var repository = new PandoRepository<TestTree>(TestTree.GenericSerializer());

			var snapshotHash = repository.SaveRootSnapshot(tree);
			var actual = repository.GetSnapshot(snapshotHash);

			await Assert.That(actual).IsNotSameReferenceAs(tree).And.IsEqualTo(tree);
		}



		[Test]
		public async Task Should_throw_if_a_root_snapshot_already_exists()
		{
			var tree = new TestTree(
				"Test Tree 1",
				new TestTree.A(1),
				new TestTree.B(
					new DateTime(1970, 01, 01),
					1000
				)
			);
			var tree2 = tree with { Name = "Different Tree" };

			var repository = new PandoRepository<TestTree>(
				TestTree.GenericSerializer()
			);
			repository.SaveRootSnapshot(tree);

			await Assert
				.That(() => repository.SaveRootSnapshot(tree2))
				.ThrowsExactly<AlreadyHasRootSnapshotException>();
		}
	}
}
