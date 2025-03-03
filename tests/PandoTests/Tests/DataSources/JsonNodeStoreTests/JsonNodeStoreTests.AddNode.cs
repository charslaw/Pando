using System.IO;
using System.Text;
using Pando.DataSources;

namespace PandoTests.Tests.DataSources.JsonNodeStoreTests;

public static partial class JsonNodeStoreTests
{
	public class AddNode
	{
		[Test]
		public async Task Should_output_node_data_to_stream()
		{
			byte[] nodeData = [0, 1, 2, 3];

			var stream = new MemoryStream();
			var dataStore = JsonNodeStore.CreateFromStream(stream);

			var nodeId = dataStore.AddNode(nodeData);

			var expected = $$"""
				{
				  "{{nodeId.ToHashString()}}": "00010203"
				}
				""";

			var actual = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
			await Assert.That(actual).IsEqualTo(expected);
		}

		[Test]
		public async Task Should_not_output_duplicate_nodes_to_stream()
		{
			byte[] nodeData = [55, 161, 83, 255];

			var stream = new MemoryStream();
			var dataStore = JsonNodeStore.CreateFromStream(stream);

			var nodeId = dataStore.AddNode(nodeData);
			_ = dataStore.AddNode(nodeData);

			var expected = $$"""
				{
				  "{{nodeId.ToHashString()}}": "37A153FF"
				}
				""";

			var actual = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
			await Assert.That(actual).IsEqualTo(expected);
		}
	}
}
