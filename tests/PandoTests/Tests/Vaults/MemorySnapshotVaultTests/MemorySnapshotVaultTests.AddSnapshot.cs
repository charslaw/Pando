using System.Collections.Generic;
using Pando.Repositories;
using Pando.Vaults;

namespace PandoTests.Tests.Vaults.MemorySnapshotVaultTests;

public static partial class MemorySnapshotVaultTests
{
	public class AddSnapshot
	{
		[Test]
		public async Task Should_add_to_snapshot_index()
		{
			var snapshotIndex = new Dictionary<SnapshotId, MemorySnapshotVault.TreeEntry>();
			var vault = new MemorySnapshotVault(snapshotIndex: snapshotIndex);

			var rootSnapshotId = vault.AddRootSnapshot(NodeId.None);
			vault.AddSnapshot(new NodeId(1), rootSnapshotId);

			await Assert.That(snapshotIndex).HasCount().EqualTo(2);
		}

		[Test]
		public async Task Should_not_throw_on_duplicate_snapshot()
		{
			var vault = new MemorySnapshotVault();

			var rootSnapshotId = vault.AddRootSnapshot(NodeId.None);

			await Assert
				.That(() =>
				{
					vault.AddSnapshot(new NodeId(1), rootSnapshotId);
					vault.AddSnapshot(new NodeId(1), rootSnapshotId);
				})
				.ThrowsNothing();
		}

		[Test]
		public async Task Should_not_add_duplicate_snapshot()
		{
			var snapshotIndex = new Dictionary<SnapshotId, MemorySnapshotVault.TreeEntry>();
			var vault = new MemorySnapshotVault(snapshotIndex: snapshotIndex);

			var rootSnapshotId = vault.AddRootSnapshot(NodeId.None);
			vault.AddSnapshot(new NodeId(1), rootSnapshotId);
			vault.AddSnapshot(new NodeId(1), rootSnapshotId);

			await Assert.That(snapshotIndex).HasCount().EqualTo(2);
		}
	}
}
