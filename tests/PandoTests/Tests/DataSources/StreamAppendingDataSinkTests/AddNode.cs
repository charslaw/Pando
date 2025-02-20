using System.IO;
using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Xunit;

namespace PandoTests.Tests.DataSources.StreamAppendingDataSinkTests;

public class AddNode
{
	[Fact]
	public void Should_output_node_index_sequentially()
	{
		var nodeIndexStream = new MemoryStream();
		using var dataSource = new StreamAppendingDataSink(
			snapshotIndexStream: Stream.Null,
			nodeIndexStream: nodeIndexStream,
			nodeDataStream: Stream.Null
		);

		dataSource.AddNode([1, 2, 3, 4]);
		dataSource.AddNode([5, 6, 7]);
		dataSource.AddNode([8, 9, 10, 11, 12]);

		byte[] expected =
		[
			..HashUtils.ComputeNodeHash([1, 2, 3, 4]).ToByteArray(),
			..ByteEncoder.GetBytes(0),
			..ByteEncoder.GetBytes(4),

			..HashUtils.ComputeNodeHash([5, 6, 7]).ToByteArray(),
			..ByteEncoder.GetBytes(4),
			..ByteEncoder.GetBytes(7),

			..HashUtils.ComputeNodeHash([8, 9, 10, 11, 12]).ToByteArray(),
			..ByteEncoder.GetBytes(7),
			..ByteEncoder.GetBytes(12)
		];

		nodeIndexStream.ToArray().Should().Equal(expected);
	}

	[Fact]
	public void Should_append_node_data_sequentially()
	{
		var nodeDataStream = new MemoryStream();
		using var dataSource = new StreamAppendingDataSink(
			snapshotIndexStream: Stream.Null,
			nodeIndexStream: Stream.Null,
			nodeDataStream: nodeDataStream
		);

		dataSource.AddNode([1, 2, 3, 4]);
		dataSource.AddNode([5, 6, 7]);
		dataSource.AddNode([8, 9, 10, 11, 12]);

		byte[] expected = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
		nodeDataStream.ToArray().Should().Equal(expected);
	}
}
