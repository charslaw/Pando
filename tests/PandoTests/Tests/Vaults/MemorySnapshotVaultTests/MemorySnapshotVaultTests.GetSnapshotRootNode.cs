using Pando.Exceptions;
using Pando.Repositories;
using Pando.Vaults;

namespace PandoTests.Tests.Vaults.MemorySnapshotVaultTests;

public static partial class MemorySnapshotVaultTests
{
	public class GetSnapshotRootNode
	{
		[Test]
		public async Task Should_return_correct_root_node_hash()
		{
			var vault = new MemorySnapshotVault();

			var snapshotId = vault.AddRootSnapshot(new NodeId(2));

			var nodeId = vault.GetSnapshotRootNodeId(snapshotId);
			await Assert.That(nodeId).IsEqualTo(new NodeId(2));
		}

		[Test]
		public async Task Should_throw_if_GetSnapshotRootNode_called_with_nonexistent_hash()
		{
			var vault = new MemorySnapshotVault();

			await Assert
				.That(() => vault.GetSnapshotRootNodeId(SnapshotId.None))
				.ThrowsExactly<SnapshotIdNotFoundException>();
		}
	}
}
