using System;
using System.IO;
using System.Text;
using Pando.DataSources;
using Pando.Repositories;

namespace PandoTests.Tests.DataSources.JsonNodeStoreTests;

public static partial class JsonNodeStoreTests
{
	public class CreateFromStream
	{
		[Test]
		public async Task Should_populate_nodes_from_given_stream()
		{
			var json = """
				{
				  "1ecc534460d8ceff": "00010203"
				}
				""";

			var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
			var store = JsonNodeStore.CreateFromStream(stream);

			var nodeId = NodeId.FromHashString("1ecc534460d8ceff");

			var size = store.GetSizeOfNode(nodeId);
			var actual = new byte[size];
			store.CopyNodeBytesTo(nodeId, actual);

			byte[] expected = [0, 1, 2, 3];
			await Assert.That(actual).IsEquivalentTo(expected);
		}
	}
}
