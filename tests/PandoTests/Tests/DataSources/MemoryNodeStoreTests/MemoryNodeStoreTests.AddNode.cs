using System;
using System.Collections.Generic;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Repositories;

namespace PandoTests.Tests.DataSources.MemoryNodeStoreTests;

public static partial class MemoryNodeStoreTests
{
	public class AddNode
	{
		[Test]
		public async Task Should_add_to_node_index_and_data()
		{
			byte[] nodeData = [0, 1, 2, 3];

			var nodeIndex = new Dictionary<NodeId, Range>();
			var nodeDataList = new SpannableList<byte>();
			var dataSource = new MemoryNodeStore(
				nodeIndex: nodeIndex,
				nodeData: nodeDataList
			);

			dataSource.AddNode([..nodeData]);

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

			var dataSource = new MemoryNodeStore();

			await Assert.That(() =>
					{
						dataSource.AddNode([..nodeData]);
						dataSource.AddNode([..nodeData]);
					}
				)
				.ThrowsNothing();
		}

		[Test]
		public async Task Should_not_add_duplicate_node_to_data_collection()
		{
			byte[] nodeData = [0, 1, 2, 3];

			var nodeDataList = new SpannableList<byte>();
			var dataSource = new MemoryNodeStore(nodeData: nodeDataList);

			dataSource.AddNode([..nodeData]);
			var preNodeDataListBytes = nodeDataList.Count;
			dataSource.AddNode([..nodeData]);
			var postNodeDataListBytes = nodeDataList.Count;

			await Assert.That(postNodeDataListBytes - preNodeDataListBytes).IsZero();
		}
	}
}
