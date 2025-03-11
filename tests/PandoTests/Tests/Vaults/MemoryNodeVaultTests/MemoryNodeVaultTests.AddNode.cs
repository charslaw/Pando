using System;
using System.Collections.Generic;
using Pando.Repositories;
using Pando.Vaults;
using Pando.Vaults.Utils;

namespace PandoTests.Tests.Vaults.MemoryNodeVaultTests;

public static partial class MemoryNodeVaultTests
{
	public class AddNode
	{
		[Test]
		public async Task Should_add_to_node_index_and_data()
		{
			byte[] nodeData = [0, 1, 2, 3];

			var nodeIndex = new Dictionary<NodeId, Range>();
			var nodeDataList = new SpannableList<byte>();
			var vault = new MemoryNodeVault(nodeIndex: nodeIndex, nodeData: nodeDataList);

			vault.AddNode([.. nodeData]);

			using (Assert.Multiple())
			{
				await Assert.That(nodeIndex).HasCount().EqualTo(1);
				await Assert.That(nodeDataList.Count).IsEqualTo(4);
			}
		}

		[Test]
		public async Task Should_not_throw_on_duplicate_node()
		{
			byte[] nodeData = [0, 1, 2, 3];

			var vault = new MemoryNodeVault();

			await Assert
				.That(() =>
				{
					vault.AddNode([.. nodeData]);
					vault.AddNode([.. nodeData]);
				})
				.ThrowsNothing();
		}

		[Test]
		public async Task Should_not_add_duplicate_node_to_data_collection()
		{
			byte[] nodeData = [0, 1, 2, 3];

			var nodeDataList = new SpannableList<byte>();
			var vault = new MemoryNodeVault(nodeData: nodeDataList);

			vault.AddNode([.. nodeData]);
			var preNodeDataListBytes = nodeDataList.Count;
			vault.AddNode([.. nodeData]);
			var postNodeDataListBytes = nodeDataList.Count;

			await Assert.That(postNodeDataListBytes - preNodeDataListBytes).IsZero();
		}
	}
}
