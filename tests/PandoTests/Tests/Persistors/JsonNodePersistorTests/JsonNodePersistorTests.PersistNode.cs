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
				{"NodeId":"1ecc534460d8ceff","Bytes":"00010203"}

				""";

			var actual = Encoding.UTF8.GetString(stream.ToArray());
			await Assert.That(actual).IsEqualTo(expected);
		}
	}
}
