using System;
using System.IO;
using System.Text;
using Pando.Persistors;
using Pando.Repositories;
using Pando.Vaults.Utils;

namespace PandoTests.Tests.Persistors.JsonSnapshotPersistorTests;

public static partial class JsonSnapshotPersistorTests
{
	public class PersistSnapshot
	{
		[Test]
		public async Task Should_write_root_snapshot_index_to_json()
		{
			var stream = new MemoryStream();
			var persistor = JsonSnapshotPersistor.CreateFromStream(stream);

			var nodeId = HashUtils.ComputeNodeHash([0, 1, 2, 3]);
			var snapshotId = HashUtils.ComputeSnapshotHash(nodeId, SnapshotId.None, SnapshotId.None);

			persistor.PersistSnapshot(snapshotId, SnapshotId.None, SnapshotId.None, nodeId);

			var expected = """
				{"SnapshotId":"5a42614c6a14f5a7","SourceParentId":null,"TargetParentId":null,"RootNodeId":"1ecc534460d8ceff"}

				""";

			var actual = Encoding.UTF8.GetString(stream.ToArray());
			await Assert.That(actual).IsEqualTo(expected);
		}

		[Test]
		public async Task Should_write_child_snapshot_to_json()
		{
			var stream = new MemoryStream();
			var persistor = JsonSnapshotPersistor.CreateFromStream(stream);

			var nodeId = HashUtils.ComputeNodeHash([0, 1, 2, 3]);
			var rootSnapshotId = HashUtils.ComputeSnapshotHash(nodeId, SnapshotId.None, SnapshotId.None);

			persistor.PersistSnapshot(rootSnapshotId, SnapshotId.None, SnapshotId.None, nodeId);

			var childSnapshotId = HashUtils.ComputeSnapshotHash(nodeId, rootSnapshotId, SnapshotId.None);
			persistor.PersistSnapshot(childSnapshotId, rootSnapshotId, SnapshotId.None, nodeId);

			var expected = """
				{"SnapshotId":"5a42614c6a14f5a7","SourceParentId":null,"TargetParentId":null,"RootNodeId":"1ecc534460d8ceff"}
				{"SnapshotId":"8e1961e8cb887c7d","SourceParentId":"5a42614c6a14f5a7","TargetParentId":null,"RootNodeId":"1ecc534460d8ceff"}

				""";

			var actual = Encoding.UTF8.GetString(stream.ToArray());
			await Assert.That(actual).IsEqualTo(expected);
		}

		[Test]
		public async Task Should_write_merge_child_snapshot_to_json()
		{
			var stream = new MemoryStream();
			var persistor = JsonSnapshotPersistor.CreateFromStream(stream);

			var nodeId = HashUtils.ComputeNodeHash([0, 1, 2, 3]);
			var sourceParentId = SnapshotId.FromHashString("5a42614c6a14f5a7");
			var targetParentId = SnapshotId.FromHashString("8e1961e8cb887c7d");
			var snapshotId = HashUtils.ComputeSnapshotHash(nodeId, sourceParentId, targetParentId);

			persistor.PersistSnapshot(snapshotId, sourceParentId, targetParentId, nodeId);

			var expected = """
				{"SnapshotId":"6133e13323a211f3","SourceParentId":"5a42614c6a14f5a7","TargetParentId":"8e1961e8cb887c7d","RootNodeId":"1ecc534460d8ceff"}

				""";

			var actual = Encoding.UTF8.GetString(stream.ToArray());
			Console.WriteLine(actual);
			await Assert.That(actual).IsEqualTo(expected);
		}
	}
}
