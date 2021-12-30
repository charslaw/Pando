using System.IO;
using FluentAssertions;
using Pando.DataSources;
using PandoTests.Utils;
using Xunit;

namespace PandoTests.Tests.DataSources;

public class PersistenceBackedDataSourceTests
{
	public class AddNode
	{
		[Fact]
		public void Should_not_output_duplicate_node_to_stream_data_source()
		{
			// Test Data
			var nodeData = new byte[] { 1, 2, 3, 4 };
			var expectedIndexLength = sizeof(ulong) + sizeof(int) + sizeof(int);
			var expectedDataLength = nodeData.Length;

			// Arrange
			var nodeIndexStream = new MemoryStream();
			var nodeDataStream = new MemoryStream();
			using var streamSource = new StreamDataSource(
				snapshotIndexStream: Stream.Null,
				leafSnapshotsStream: Stream.Null,
				nodeIndexStream: nodeIndexStream,
				nodeDataStream: nodeDataStream
			);
			var memorySource = new MemoryDataSource();
			var dataSource = new PersistenceBackedDataSource(memorySource, streamSource);

			// Act
			dataSource.AddNode(nodeData.CreateCopy());
			dataSource.AddNode(nodeData.CreateCopy());

			// Assert
			var actualIndex = nodeIndexStream.ToArray();
			var actualData = nodeDataStream.ToArray();

			actualIndex.Length.Should().Be(expectedIndexLength);
			actualData.Length.Should().Be(expectedDataLength);
		}
	}

	public class AddSnapshot
	{
		[Fact]
		public void Should_not_output_duplicate_snapshot_to_stream_data_source()
		{
			// Test Data
			var parentHash = 5UL;
			var rootNodeHash = 27UL;
			var expectedIndexLength = sizeof(ulong) * 3;

			// Arrange
			var snapshotIndexStream = new MemoryStream();
			using var streamSource = new StreamDataSource(
				snapshotIndexStream: snapshotIndexStream,
				leafSnapshotsStream: Stream.Null,
				nodeIndexStream: Stream.Null,
				nodeDataStream: Stream.Null
			);
			var memoryDataSource = new MemoryDataSource();
			var dataSource = new PersistenceBackedDataSource(memoryDataSource, streamSource);

			// Act
			dataSource.AddSnapshot(parentHash, rootNodeHash);
			dataSource.AddSnapshot(parentHash, rootNodeHash);

			// Assert
			var actualIndex = snapshotIndexStream.ToArray();

			actualIndex.Length.Should().Be(expectedIndexLength);
		}
	}
}
