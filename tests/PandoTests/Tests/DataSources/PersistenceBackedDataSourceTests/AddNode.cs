using System.IO;
using FluentAssertions;
using Pando.DataSources;
using PandoTests.Utils;
using Xunit;

namespace PandoTests.Tests.DataSources.PersistenceBackedDataSourceTests;

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
		using var streamSource = new StreamAppendingDataSink(
			snapshotIndexStream: Stream.Null,
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
