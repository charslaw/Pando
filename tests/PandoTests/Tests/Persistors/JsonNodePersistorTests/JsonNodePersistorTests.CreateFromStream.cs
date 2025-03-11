using System.IO;
using System.Text;
using Pando.Persistors;
using Pando.Repositories;

namespace PandoTests.Tests.Persistors.JsonNodePersistorTests;

public static partial class JsonNodePersistorTests
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
			var persistor = JsonNodePersistor.CreateFromStream(stream);

			var nodeId = NodeId.FromHashString("1ecc534460d8ceff");

			var actual = persistor.NodeIndex[nodeId];
			byte[] expected = [0, 1, 2, 3];
			await Assert.That(actual).IsEquivalentTo(expected);
		}
	}
}
