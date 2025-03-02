using System;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Exceptions;
using Pando.Repositories;

namespace PandoTests.Tests.DataSources.MemoryNodeStoreTests;

public static partial class MemoryNodeStoreTests
{
	public class CopyNodeBytesTo
	{
		[Test]
		public async Task Should_get_added_node()
		{
			byte[] nodeData = [0, 1, 2, 3];
			var nodeId = HashUtils.ComputeNodeHash(nodeData);

			var dataSource = new MemoryNodeStore();
			dataSource.AddNode([..nodeData]);

			var actual = new byte[4];
			dataSource.CopyNodeBytesTo(nodeId, actual);

			await Assert.That(actual).IsEquivalentTo(nodeData);
		}

		[Test]
		public async Task Should_return_correct_data_when_multiple_nodes_exist()
		{
			byte[] nodeData1 = [0, 1, 2, 3];
			byte[] nodeData2 = [4, 5, 6, 7];
			byte[] nodeData3 = [8, 9, 10, 11];
			var node2Id = HashUtils.ComputeNodeHash(nodeData2);

			var dataSource = new MemoryNodeStore();
			dataSource.AddNode([..nodeData1]);
			dataSource.AddNode([..nodeData2]);
			dataSource.AddNode([..nodeData3]);

			var actual = new byte[4];
			dataSource.CopyNodeBytesTo(node2Id, actual);

			await Assert.That(actual).IsEquivalentTo(nodeData2);
		}

		[Test]
		public async Task Should_throw_if_called_with_nonexistent_hash()
		{
			var dataSource = new MemoryNodeStore();

			await Assert.That(() =>
					{
						Span<byte> buffer = [];
						dataSource.CopyNodeBytesTo(NodeId.None, buffer);
					}
				)
				.ThrowsExactly<NodeIdNotFoundException>();
		}
	}
}
