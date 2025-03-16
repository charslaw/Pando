using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Pando.Persistors;
using Pando.Repositories;

namespace PandoTests.Tests.Persistors.JsonSnapshotPersistorTests;

public static partial class JsonSnapshotPersistorTests
{
	public class LoadSnapshotIndex
	{
		[Test]
		public async Task Should_return_empty_if_stream_is_empty()
		{
			var stream = new MemoryStream();
			var persistor = JsonSnapshotPersistor.CreateFromStream(stream);

			var actual = persistor.LoadSnapshotIndex();

			await Assert
				.That(actual)
				.IsEquivalentTo(Enumerable.Empty<KeyValuePair<SnapshotId, (SnapshotId, SnapshotId, NodeId)>>());
		}

		[Test]
		public async Task Should_return_data_from_pre_populated_stream()
		{
			var json = """
				{
				  "5a42614c6a14f5a7": {
				    "SourceParentId": null,
				    "TargetParentId": null,
				    "RootNodeId": "1ecc534460d8ceff"
				  }
				}
				""";

			var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
			var persistor = JsonSnapshotPersistor.CreateFromStream(stream);

			var actual = persistor.LoadSnapshotIndex();

			IEnumerable<KeyValuePair<SnapshotId, (SnapshotId, SnapshotId, NodeId)>> expected =
			[
				new(
					SnapshotId.FromHashString("5a42614c6a14f5a7"),
					(SnapshotId.None, SnapshotId.None, NodeId.FromHashString("1ecc534460d8ceff"))
				),
			];

			await Assert.That(actual).IsEquivalentTo(expected);
		}

		[Test]
		public async Task Should_return_data_from_stream_altered_after_creation()
		{
			var stream = new MemoryStream();
			var persistor = JsonSnapshotPersistor.CreateFromStream(stream);

			var json = """
				{
				  "5a42614c6a14f5a7": {
				    "SourceParentId": null,
				    "TargetParentId": null,
				    "RootNodeId": "1ecc534460d8ceff"
				  }
				}
				""";

			stream.Write(Encoding.UTF8.GetBytes(json));

			var actual = persistor.LoadSnapshotIndex();

			IEnumerable<KeyValuePair<SnapshotId, (SnapshotId, SnapshotId, NodeId)>> expected =
			[
				new(
					SnapshotId.FromHashString("5a42614c6a14f5a7"),
					(SnapshotId.None, SnapshotId.None, NodeId.FromHashString("1ecc534460d8ceff"))
				),
			];

			await Assert.That(actual).IsEquivalentTo(expected);
		}
	}
}
