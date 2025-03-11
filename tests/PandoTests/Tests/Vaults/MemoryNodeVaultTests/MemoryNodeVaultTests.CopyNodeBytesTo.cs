using System;
using Pando.Exceptions;
using Pando.Repositories;
using Pando.Vaults;
using Pando.Vaults.Utils;

namespace PandoTests.Tests.Vaults.MemoryNodeVaultTests;

public static partial class MemoryNodeVaultTests
{
	public class CopyNodeBytesTo
	{
		[Test]
		public async Task Should_get_added_node()
		{
			byte[] nodeData = [0, 1, 2, 3];
			var nodeId = HashUtils.ComputeNodeHash(nodeData);

			var vault = new MemoryNodeVault();
			vault.AddNode([.. nodeData]);

			var actual = new byte[4];
			vault.CopyNodeBytesTo(nodeId, actual);

			await Assert.That(actual).IsEquivalentTo(nodeData);
		}

		[Test]
		public async Task Should_return_correct_data_when_multiple_nodes_exist()
		{
			byte[] nodeData1 = [0, 1, 2, 3];
			byte[] nodeData2 = [4, 5, 6, 7];
			byte[] nodeData3 = [8, 9, 10, 11];
			var node2Id = HashUtils.ComputeNodeHash(nodeData2);

			var vault = new MemoryNodeVault();
			vault.AddNode([.. nodeData1]);
			vault.AddNode([.. nodeData2]);
			vault.AddNode([.. nodeData3]);

			var actual = new byte[4];
			vault.CopyNodeBytesTo(node2Id, actual);

			await Assert.That(actual).IsEquivalentTo(nodeData2);
		}

		[Test]
		public async Task Should_throw_if_called_with_nonexistent_hash()
		{
			var vault = new MemoryNodeVault();

			await Assert
				.That(() =>
				{
					Span<byte> buffer = [];
					vault.CopyNodeBytesTo(NodeId.None, buffer);
				})
				.ThrowsExactly<NodeIdNotFoundException>();
		}
	}
}
