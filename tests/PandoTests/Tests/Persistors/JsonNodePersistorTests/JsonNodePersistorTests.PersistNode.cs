using System.IO;
using System.Text;
using Pando.Persistors;
using Pando.Vaults.Utils;

namespace PandoTests.Tests.Persistors.JsonNodePersistorTests;

public static partial class JsonNodePersistorTests
{
	public class PersistNode
	{
		[Test]
		public async Task Should_output_node_data_to_stream()
		{
			byte[] nodeData = [0, 1, 2, 3];

			var stream = new MemoryStream();
			var persistor = JsonNodePersistor.CreateFromStream(stream);

			var nodeId = HashUtils.ComputeNodeHash(nodeData);
			persistor.PersistNode(nodeId, nodeData);

			var expected = """
				{
				  "1ecc534460d8ceff": "00010203"
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
			var persistor = JsonNodePersistor.CreateFromStream(stream);

			var nodeId = HashUtils.ComputeNodeHash(nodeData);
			persistor.PersistNode(nodeId, nodeData);
			persistor.PersistNode(nodeId, nodeData);

			var expected = """
				{
				  "2860d87ea6df310b": "37A153FF"
				}
				""";

			var actual = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
			await Assert.That(actual).IsEqualTo(expected);
		}
	}
}
