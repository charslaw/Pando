using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using PandoTests.Utils;
using Standart.Hash.xxHash;
using Xunit;

namespace PandoTests.Tests.DataSources;

public class StreamDataSourceTests
{
	public class AddNode
	{
		[Fact]
		public void Should_output_node_index_sequentially()
		{
			var nodeIndexStream = new MemoryStream();
			using var dataSource = new StreamDataSource(
				snapshotIndexStream: Stream.Null,
				leafSnapshotsStream: Stream.Null,
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
			using var dataSource = new StreamDataSource(
				snapshotIndexStream: Stream.Null,
				leafSnapshotsStream: Stream.Null,
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

	public class AddSnapshot
	{
		[Fact]
		public void Should_output_snapshot_index_when_root_snapshot_added()
		{
			// Test Data
			ulong rootNodeHash = 0xFF_AA_00_BB_FF_CC_00_DD;

			// Arrange
			var snapshotIndexStream = new MemoryStream();
			using var dataSource = new StreamDataSource(
				snapshotIndexStream: snapshotIndexStream,
				leafSnapshotsStream: Stream.Null,
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
			using var dataSource = new StreamDataSource(
				snapshotIndexStream: snapshotIndexStream,
				leafSnapshotsStream: Stream.Null,
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

		[Fact]
		public void Should_update_leaf_nodes_when_snapshot_added()
		{
			// Arrange
			var leafSnapshotsStream = new MemoryStream();
			using var dataSource = new StreamDataSource(
				snapshotIndexStream: Stream.Null,
				leafSnapshotsStream: leafSnapshotsStream,
				nodeIndexStream: Stream.Null,
				nodeDataStream: Stream.Null
			);

			// Act
			var snapshotHash = dataSource.AddSnapshot(SnapshotId.None, new NodeId(1));

			// Assert
			var leafSnapshotsRaw = leafSnapshotsStream.ToArray();
			leafSnapshotsRaw.Length.Should().Be(8);
			var actualHash = SnapshotId.FromBuffer(leafSnapshotsRaw);
			actualHash.Should().Be(snapshotHash);
		}

		[Fact]
		public void Should_update_leaf_snapshot_stream_when_child_added()
		{
			// Arrange
			var leafSnapshotsStream = new MemoryStream();
			using var dataSource = new StreamDataSource(
				snapshotIndexStream: Stream.Null,
				leafSnapshotsStream: leafSnapshotsStream,
				nodeIndexStream: Stream.Null,
				nodeDataStream: Stream.Null
			);

			// Act
			var rootSnapshotId = dataSource.AddSnapshot(SnapshotId.None, new NodeId(1));
			var childId = dataSource.AddSnapshot(rootSnapshotId, new NodeId(2));

			// Assert
			var leafSnapshotsRaw = leafSnapshotsStream.ToArray();
			leafSnapshotsRaw.Length.Should().Be(8);
			var actualHash = SnapshotId.FromBuffer(leafSnapshotsRaw);
			actualHash.Should().Be(childId);
		}

		[Fact]
		public void Should_update_stream_to_contain_leaf_nodes_of_all_branches()
		{
			// Arrange
			var leafSnapshotsStream = new MemoryStream();
			using var dataSource = new StreamDataSource(
				snapshotIndexStream: Stream.Null,
				leafSnapshotsStream: leafSnapshotsStream,
				nodeIndexStream: Stream.Null,
				nodeDataStream: Stream.Null
			);

			// Act
			var rootSnapshotId = dataSource.AddSnapshot(SnapshotId.None, new NodeId(1));
			var child1Hash = dataSource.AddSnapshot(rootSnapshotId, new NodeId(2));
			var child2Hash = dataSource.AddSnapshot(rootSnapshotId, new NodeId(3));

			// Assert
			var leafSnapshotsRaw = leafSnapshotsStream.ToArray();
			leafSnapshotsRaw.Length.Should().Be(16);
			var actualHashes = GetHashesFromByteArray(leafSnapshotsRaw);
			actualHashes.Should().BeEquivalentTo(new[] { child1Hash, child2Hash });
		}
	}

	public class ReonstitutionConstructor
	{
		[Fact]
		public void Should_account_for_existing_leaf_snapshots()
		{
			// Test Data
			var snapshot1Id = new SnapshotId(1UL);
			var snapshot2Id = new SnapshotId(2UL);
			var initialLeafSnapshots = new byte[SnapshotId.SIZE * 2];
			snapshot1Id.CopyTo(initialLeafSnapshots.AsSpan(0, sizeof(ulong)));
			snapshot2Id.CopyTo(initialLeafSnapshots.AsSpan(sizeof(ulong), sizeof(ulong)));

			// Arrange
			var leafSnapshotsStream = new MemoryStream(initialLeafSnapshots);
			using var dataSource = new StreamDataSource(
				snapshotIndexStream: Stream.Null,
				leafSnapshotsStream: leafSnapshotsStream,
				nodeIndexStream: Stream.Null,
				nodeDataStream: Stream.Null
			);

			// Act
			var newSnapshot = dataSource.AddSnapshot(snapshot1Id, NodeId.None);

			// Assert
			var actualHashes = GetHashesFromByteArray(leafSnapshotsStream.ToArray());
			actualHashes.Should().BeEquivalentTo(new[] { newSnapshot, snapshot2Id });
		}
	}


	private static IEnumerable<SnapshotId> GetHashesFromByteArray(byte[] input)
	{
		for (int i = 0; i < input.Length; i += 8)
		{
			yield return SnapshotId.FromBuffer(input.AsSpan(i, SnapshotId.SIZE));
		}
	}
}
