using System;
using System.Collections.Generic;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Exceptions;
using Pando.Repositories;
using PandoTests.Tests.Repositories.TestStateTrees;

namespace PandoTests.Tests.Repositories.PandoRepositoryTests;

public static partial class PandoRepositoryTests
{
	public class SaveSnapshot
	{
		[Test]
		public async Task Should_create_a_new_snapshot_even_if_state_tree_is_equivalent()
		{
			var tree1 = new TestTree(
				"Test Tree 1",
				new TestTree.A(1),
				new TestTree.B(new DateTime(1970, 01, 01), 1000)
			);
			var tree1Copy = tree1 with { };

			var repository = new PandoRepository<TestTree>(TestTree.GenericSerializer());

			var snapshotHash1 = repository.SaveRootSnapshot(tree1);
			var snapshotHash2 = repository.SaveSnapshot(tree1Copy, snapshotHash1);

			await Assert.That(snapshotHash1).IsNotEqualTo(snapshotHash2);
		}

		[Test]
		public async Task Should_not_create_duplicate_blobs_when_equivalent_state_tree_is_added()
		{
			var tree = new TestTree("Test Tree 1", new TestTree.A(1), new TestTree.B(new DateTime(1970, 01, 01), 1000));
			var tree2 = tree with { };

			var nodeIndex = new Dictionary<NodeId, Range>();
			var nodeData = new SpannableList<byte>();
			var repository = new PandoRepository<TestTree>(
				new MemoryNodeStore(nodeIndex: nodeIndex, nodeData: nodeData),
				new MemorySnapshotStore(),
				TestTree.GenericSerializer()
			);

			var rootHash = repository.SaveRootSnapshot(tree);
			repository.SaveSnapshot(tree2, rootHash);

			await Assert.That(nodeIndex).HasCount().EqualTo(4);
		}

		[Test]
		public async Task Should_throw_if_given_parent_snapshot_hash_doesnt_exist()
		{
			var tree1 = new TestTree(
				"Test Tree 1",
				new TestTree.A(1),
				new TestTree.B(new DateTime(1970, 01, 01), 1000)
			);

			var source = new MemorySnapshotStore();
			var repository = new PandoRepository<TestTree>(new MemoryNodeStore(), source, TestTree.GenericSerializer());

			using (Assert.Multiple())
			{
				await Assert
					.That(() => repository.SaveSnapshot(tree1, SnapshotId.None))
					.ThrowsExactly<SnapshotIdNotFoundException>();
				await Assert.That(source.SnapshotCount).IsEqualTo(0);
			}
		}
	}
}
