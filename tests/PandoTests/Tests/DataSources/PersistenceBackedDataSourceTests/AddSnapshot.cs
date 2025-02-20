using System.IO;
using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Xunit;

namespace PandoTests.Tests.DataSources.PersistenceBackedDataSourceTests;

public class AddSnapshot
{
	[Fact]
	public void Should_not_output_duplicate_snapshot_to_stream_data_source()
	{
		// Test Data
		var parentSnapshotId = new SnapshotId(5);
		var rootNodeId = new NodeId(27);
		var expectedIndexLength = SnapshotId.SIZE + SnapshotId.SIZE + NodeId.SIZE;

		// Arrange
		var snapshotIndexStream = new MemoryStream();
		using var streamSource = new StreamAppendingDataSink(
			snapshotIndexStream: snapshotIndexStream,
			nodeIndexStream: Stream.Null,
			nodeDataStream: Stream.Null
		);
		var memoryDataSource = new MemoryDataSource();
		var dataSource = new PersistenceBackedDataSource(memoryDataSource, streamSource);

		// Act
		dataSource.AddSnapshot(parentSnapshotId, rootNodeId);
		dataSource.AddSnapshot(parentSnapshotId, rootNodeId);

		// Assert
		var actualIndex = snapshotIndexStream.ToArray();

		actualIndex.Length.Should().Be(expectedIndexLength);
	}
}
