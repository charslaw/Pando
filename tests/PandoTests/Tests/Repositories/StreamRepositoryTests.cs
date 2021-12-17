using System;
using System.IO;
using FluentAssertions;
using Pando.Repositories;
using Pando.Repositories.Utils;
using PandoTests.Utils;
using Standart.Hash.xxHash;
using Xunit;

namespace PandoTests.Tests.Repositories;

public class StreamRepositoryTests
{
	public class AddNode
	{
		[Fact]
		public void Should_output_node_index_sequentially()
		{
			// Test Data
			var nodeData = new[]
			{
				new byte[] { 1, 2, 3, 4 },
				new byte[] { 5, 6, 7 },
				new byte[] { 8, 9, 10, 11, 12 }
			};

			// Arrange
			var nodeIndexStream = new MemoryStream();
			using var repository = new StreamRepository(Stream.Null, nodeIndexStream, Stream.Null);

			// Act
			repository.AddNode(nodeData[0]);
			repository.AddNode(nodeData[1]);
			repository.AddNode(nodeData[2]);

			// Assert
			Span<byte> expected = stackalloc byte[48];
			ByteEncoder.CopyBytes(xxHash64.ComputeHash(nodeData[0], nodeData[0].Length), expected.Slice(0, 8));
			ByteEncoder.CopyBytes(0, expected.Slice(8, 4));
			ByteEncoder.CopyBytes(nodeData[0].Length, expected.Slice(12, 4));
			ByteEncoder.CopyBytes(xxHash64.ComputeHash(nodeData[1], nodeData[1].Length), expected.Slice(16, 8));
			ByteEncoder.CopyBytes(nodeData[0].Length, expected.Slice(24, 4));
			ByteEncoder.CopyBytes(nodeData[1].Length, expected.Slice(28, 4));
			ByteEncoder.CopyBytes(xxHash64.ComputeHash(nodeData[2], nodeData[2].Length), expected.Slice(32, 8));
			ByteEncoder.CopyBytes(nodeData[0].Length + nodeData[1].Length, expected.Slice(40, 4));
			ByteEncoder.CopyBytes(nodeData[2].Length, expected.Slice(44, 4));

			nodeIndexStream.ToArray().Should().Equal(expected.ToArray());
		}

		[Fact]
		public void Should_append_node_data_sequentially()
		{
			// Test Data
			var nodeData1 = new byte[] { 1, 2, 3, 4 };
			var nodeData2 = new byte[] { 5, 6, 7 };
			var nodeData3 = new byte[] { 8, 9, 10, 11, 12 };

			// Arrange
			var nodeDataStream = new MemoryStream();
			using var repository = new StreamRepository(Stream.Null, Stream.Null, nodeDataStream);

			// Act
			repository.AddNode(nodeData1);
			repository.AddNode(nodeData2);
			repository.AddNode(nodeData3);

			// Assert
			var expected = ArrayX.Concat(nodeData1, nodeData2, nodeData3);
			nodeDataStream.ToArray().Should().Equal(expected);
		}
	}

	public class AddSnapshot
	{
		[Fact]
		public void Should_output_snapshot_index_when_root_snapshot_added()
		{
			// Test Data
			ulong rootNodeHash = 0xFF_AA_00_BB_FF_CC_00_DD;
			var expectedIndex = new byte[]
			{
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // parent hash
				0xDD, 0x00, 0xCC, 0xFF, 0xBB, 0x00, 0xAA, 0xFF  // root node hash
			};
			var expectedHash = xxHash64.ComputeHash(expectedIndex);

			// Arrange
			var snapshotIndexStream = new MemoryStream();
			using var repository = new StreamRepository(snapshotIndexStream, Stream.Null, Stream.Null);

			// Act
			repository.AddSnapshot(0, rootNodeHash);

			// Assert
			var snapshotIndex = snapshotIndexStream.ToArray();
			var actualHash = ByteEncoder.GetUInt64(snapshotIndex);
			var actualIndex = snapshotIndex[8..];
			actualIndex.Should().Equal(expectedIndex);
			actualHash.Should().Be(expectedHash);
		}

		[Fact]
		public void Should_output_snapshot_index_when_snapshot_added()
		{
			// Test Data
			ulong parentHash = 0x12_34_56_78_90_AB_CD_EF;
			ulong rootNodeHash = 0xFF_AA_00_BB_FF_CC_00_DD;
			var expectedIndex = new byte[]
			{
				0xEF, 0xCD, 0xAB, 0x90, 0x78, 0x56, 0x34, 0x12, // parent hash
				0xDD, 0x00, 0xCC, 0xFF, 0xBB, 0x00, 0xAA, 0xFF  // root node hash
			};
			var expectedHash = xxHash64.ComputeHash(expectedIndex);

			// Arrange
			var snapshotIndexStream = new MemoryStream();
			using var repository = new StreamRepository(snapshotIndexStream, Stream.Null, Stream.Null);

			// Act
			repository.AddSnapshot(parentHash, rootNodeHash);

			// Assert
			var snapshotIndex = snapshotIndexStream.ToArray();
			var actualHash = ByteEncoder.GetUInt64(snapshotIndex);
			var actualIndex = snapshotIndex[8..];
			actualIndex.Should().Equal(expectedIndex);
			actualHash.Should().Be(expectedHash);
		}
	}
}
