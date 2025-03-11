using Pando.Repositories;
using Pando.Vaults;

namespace PandoTests.Tests.Vaults.MemorySnapshotVaultTests;

public static partial class MemorySnapshotVaultTests
{
	public class GetSnapshotLeastCommonAncestor
	{
		[Test]
		public async Task Should_return_correct_least_common_ancestor_snapshot_hash()
		{
			var vault = new MemorySnapshotVault();

			var rootId = vault.AddRootSnapshot(new NodeId(1));
			var childId = vault.AddSnapshot(new NodeId(2), rootId);
			var branch1Id = vault.AddSnapshot(new NodeId(3), childId);
			var branch2AId = vault.AddSnapshot(new NodeId(4), childId);
			var branch2BId = vault.AddSnapshot(new NodeId(5), branch2AId);

			var lca = vault.GetSnapshotLeastCommonAncestor(branch1Id, branch2BId);

			await Assert.That(lca).IsEqualTo(childId);
		}

		[Test]
		public async Task Should_return_correct_lca_when_first_snapshot_is_ancestor_of_second()
		{
			var vault = new MemorySnapshotVault();
			var rootId = vault.AddRootSnapshot(new NodeId(1));
			var childId = vault.AddSnapshot(new NodeId(2), rootId);
			var grandchildId = vault.AddSnapshot(new NodeId(3), childId);

			var lca = vault.GetSnapshotLeastCommonAncestor(childId, grandchildId);

			await Assert.That(lca).IsEqualTo(childId);
		}

		[Test]
		public async Task Should_return_correct_lca_when_second_snapshot_is_ancestor_of_first()
		{
			var vault = new MemorySnapshotVault();
			var rootId = vault.AddRootSnapshot(new NodeId(1));
			var childId = vault.AddSnapshot(new NodeId(2), rootId);
			var grandchildId = vault.AddSnapshot(new NodeId(3), childId);

			var lca = vault.GetSnapshotLeastCommonAncestor(grandchildId, childId);

			await Assert.That(lca).IsEqualTo(childId);
		}
	}
}
