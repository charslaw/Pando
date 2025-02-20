using System;
using System.IO;
using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using PandoTests.Utils;
using Xunit;

namespace PandoTests.Tests.DataSources.MemoryDataSourceTests;

public class ReconstitutionConstructor
{
	[Fact]
	public void Should_initialize_data_source_with_node()
	{
		// Test Data
		var nodeId = new NodeId(123);
		var nodeIndex = new byte[16];
		nodeId.CopyTo(nodeIndex.AsSpan()[..8]);

		// Arrange/Act
		var nodeIndexStream = new MemoryStream(nodeIndex.CreateCopy());
		var dataSource = new MemoryDataSource(
			snapshotIndexSource: Stream.Null,
			nodeIndexSource: nodeIndexStream,
			nodeDataSource: Stream.Null
		);

		// Assert
		dataSource.HasNode(nodeId).Should().BeTrue();
	}

	[Fact]
	public void Should_initialize_data_source_with_correct_node_data()
	{
		// Test Data
		var id1 = new NodeId(0123);
		var id2 = new NodeId(4567);
		byte[] nodeIndexEntry = [
			..id1.ToByteArray(),
			..ByteEncoder.GetBytes(0),
			..ByteEncoder.GetBytes(4),
			..id2.ToByteArray(),
			..ByteEncoder.GetBytes(4),
			..ByteEncoder.GetBytes(8)
		];

		// Arrange/Act
		var nodeIndexStream = new MemoryStream(nodeIndexEntry);
		var nodeDataStream = new MemoryStream([0, 1, 2, 3, 4, 5, 6, 7]);
		var dataSource = new MemoryDataSource(
			snapshotIndexSource: Stream.Null,
			nodeIndexSource: nodeIndexStream,
			nodeDataSource: nodeDataStream
		);

		// Assert
		byte[] actualBuffer = new byte[8];
		dataSource.CopyNodeBytesTo(id1, actualBuffer.AsSpan()[..4]);
		dataSource.CopyNodeBytesTo(id2, actualBuffer.AsSpan()[4..]);
		byte[] expected = [0, 1, 2, 3, 4, 5, 6, 7];
		actualBuffer.Should().Equal(expected);
	}

	[Fact]
	public void Should_initialize_data_source_with_snapshot()
	{
		// Test Data
		var snapshotId = new SnapshotId(123);
		var snapshotIndex = new byte[24];
		snapshotId.CopyTo(snapshotIndex.AsSpan()[..8]);

		// Arrange/Act
		var snapshotIndexStream = new MemoryStream(snapshotIndex);
		var dataSource = new MemoryDataSource(
			snapshotIndexSource: snapshotIndexStream,
			nodeIndexSource: Stream.Null,
			nodeDataSource: Stream.Null
		);

		// Assert
		dataSource.HasSnapshot(snapshotId).Should().BeTrue();
	}

	[Fact]
	public void Should_initialize_data_source_with_correct_snapshot_data()
	{
		// Test Data
		var snapshotId = new SnapshotId(123);
		var parentSnapshotId = new SnapshotId(5);
		var rootNodeId = new NodeId(42);
		byte[] snapshotIndexEntry = [..snapshotId.ToByteArray(), ..parentSnapshotId.ToByteArray(), ..rootNodeId.ToByteArray()];

		// Arrange/Act
		var snapshotIndexStream = new MemoryStream(snapshotIndexEntry.CreateCopy());
		var dataSource = new MemoryDataSource(
			snapshotIndexSource: snapshotIndexStream,
			nodeIndexSource: Stream.Null,
			nodeDataSource: Stream.Null
		);

		// Assert
		var actualParentHash = dataSource.GetSnapshotParent(snapshotId);
		var actualRootNodeHash = dataSource.GetSnapshotRootNode(snapshotId);
		actualParentHash.Should().Be(parentSnapshotId);
		actualRootNodeHash.Should().Be(rootNodeId);
	}

	[Fact]
	public void Should_not_overallocate_node_data_list()
	{
		var nodeDataStream = new MemoryStream(8);
		nodeDataStream.Write(new byte[] { 1, 2, 3 });
		var nodeDataList = new SpannableList<byte>();
		_ = new MemoryDataSource(
			snapshotIndexSource: Stream.Null,
			nodeIndexSource: Stream.Null,
			nodeDataSource: nodeDataStream,
			nodeData: nodeDataList
		);

		nodeDataList.Count.Should().Be(3);
	}
}
