using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Pando.Persistors;
using Pando.Repositories;

namespace PandoTests.Tests.Persistors.JsonNodePersistorTests;

public static partial class JsonNodePersistorTests
{
	public class LoadNodeData
	{
		[Test]
		public async Task Should_return_empty_if_stream_is_empty()
		{
			var stream = new MemoryStream();
			var persistor = JsonNodePersistor.CreateFromStream(stream);

			var actual = persistor.LoadNodeData();

			await Assert
				.That(actual)
				.IsEquivalentTo((Enumerable.Empty<KeyValuePair<NodeId, Range>>(), Enumerable.Empty<byte>()));
		}

		[Test]
		public async Task Should_return_data_from_pre_populated_stream()
		{
			var json = """
				{
				  "1ecc534460d8ceff": "00010203"
				}
				""";

			var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
			var persistor = JsonNodePersistor.CreateFromStream(stream);

			var actual = persistor.LoadNodeData();

			(Dictionary<NodeId, Range>, byte[]) expected = (
				new Dictionary<NodeId, Range> { { NodeId.FromHashString("1ecc534460d8ceff"), 0..4 } },
				[0, 1, 2, 3]
			);
			await Assert.That(actual).IsEquivalentTo(expected);
		}

		[Test]
		public async Task Should_return_data_from_stream_altered_after_creation()
		{
			var stream = new MemoryStream();
			var persistor = JsonNodePersistor.CreateFromStream(stream);

			var json = """
				{
				  "1ecc534460d8ceff": "00010203"
				}
				""";

			stream.Write(Encoding.UTF8.GetBytes(json));

			var actual = persistor.LoadNodeData();

			(Dictionary<NodeId, Range>, byte[]) expected = (
				new Dictionary<NodeId, Range> { { NodeId.FromHashString("1ecc534460d8ceff"), 0..4 } },
				[0, 1, 2, 3]
			);
			await Assert.That(actual).IsEquivalentTo(expected);
		}
	}
}
