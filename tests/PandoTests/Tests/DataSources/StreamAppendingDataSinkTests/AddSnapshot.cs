using System.Buffers.Binary;
using System.IO;
using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Standart.Hash.xxHash;
using Xunit;

namespace PandoTests.Tests.DataSources.StreamAppendingDataSinkTests;

public class AddSnapshot
{
	[Fact]
	public void Should_output_snapshot_index_when_root_snapshot_added()
	{
		// Test Data
		ulong rootNodeHash = 0xFF_AA_00_BB_FF_CC_00_DD;

		// Arrange
		var snapshotIndexStream = new MemoryStream();
		using var dataSource = new StreamAppendingDataSink(
			snapshotIndexStream: snapshotIndexStream,
			nodeIndexStream: Stream.Null,
			nodeDataStream: Stream.Null
		);

		// Act
		dataSource.AddSnapshot(SnapshotId.None, new NodeId(rootNodeHash));

		// Assert
		var snapshotIndex = snapshotIndexStream.ToArray();
		var actualSnapshotId = BinaryPrimitives.ReadUInt64LittleEndian(snapshotIndex);
		var actualIndex = snapshotIndex[8..];

		var expectedSnapshotIndex = new byte[]
		{
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // parent hash
			0xDD, 0x00, 0xCC, 0xFF, 0xBB, 0x00, 0xAA, 0xFF  // root node hash
		};
		ulong expectedSnapshotId = xxHash64.ComputeHash(expectedSnapshotIndex);

		actualIndex.Should().Equal(expectedSnapshotIndex);
		actualSnapshotId.Should().Be(expectedSnapshotId);
	}

	[Fact]
	public void Should_output_snapshot_index_when_snapshot_added()
	{
		// Test Data
		ulong parentHash = 0x12_34_56_78_90_AB_CD_EF;
		ulong rootNodeHash = 0xFF_AA_00_BB_FF_CC_00_DD;

		// Arrange
		var snapshotIndexStream = new MemoryStream();
		using var dataSource = new StreamAppendingDataSink(
			snapshotIndexStream: snapshotIndexStream,
			nodeIndexStream: Stream.Null,
			nodeDataStream: Stream.Null
		);

		// Act
		dataSource.AddSnapshot(new SnapshotId(parentHash), new NodeId(rootNodeHash));

		// Assert
		var snapshotIndex = snapshotIndexStream.ToArray();
		var actualSnapshotHash = BinaryPrimitives.ReadUInt64LittleEndian(snapshotIndex);
		var actualSnapshotIndex = snapshotIndex[8..];

		var expectedSnapshotIndex = new byte[]
		{
			0xEF, 0xCD, 0xAB, 0x90, 0x78, 0x56, 0x34, 0x12, // parent hash
			0xDD, 0x00, 0xCC, 0xFF, 0xBB, 0x00, 0xAA, 0xFF  // root node hash
		};
		var expectedSnapshotHash = xxHash64.ComputeHash(expectedSnapshotIndex);

		actualSnapshotIndex.Should().Equal(expectedSnapshotIndex);
		actualSnapshotHash.Should().Be(expectedSnapshotHash);
	}


}
