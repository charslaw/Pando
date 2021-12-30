using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Pando;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Exceptions;
using PandoTests.Utils;
using Standart.Hash.xxHash;
using Xunit;

namespace PandoTests.Tests.DataSources;

public class MemoryDataSourceTests
{
	public class AddNode
	{
		[Fact]
		public void Should_add_to_node_index_and_data()
		{
			// Test Data
			var nodeData = new byte[] { 0, 1, 2, 3 };

			// Arrange
			var nodeIndex = new Dictionary<ulong, DataSlice>();
			var nodeDataList = new SpannableList<byte>();
			var dataSource = new MemoryDataSource(
				nodeIndex: nodeIndex,
				nodeData: nodeDataList
			);

			// Act
			dataSource.AddNode(nodeData.CreateCopy());

			// Act
			nodeIndex.Count.Should().Be(1);
			nodeDataList.Count.Should().Be(4);
		}

		[Fact]
		public void Should_not_throw_on_duplicate_node()
		{
			// Test Data
			var nodeData = new byte[] { 0, 1, 2, 3 };

			// Arrange
			var dataSource = new MemoryDataSource();

			// Assert
			dataSource.Invoking(source =>
					{
						source.AddNode(nodeData.CreateCopy());
						source.AddNode(nodeData.CreateCopy());
					}
				)
				.Should()
				.NotThrow();
		}

		[Fact]
		public void Should_not_add_duplicate_node_to_data_collection()
		{
			// Test Data
			var nodeData = new byte[] { 0, 1, 2, 3 };

			// Arrange
			var nodeDataList = new SpannableList<byte>();
			var dataSource = new MemoryDataSource(nodeData: nodeDataList);

			// Act
			dataSource.AddNode(nodeData.CreateCopy());
			var preNodeDataListBytes = nodeDataList.Count;
			dataSource.AddNode(nodeData.CreateCopy());
			var postNodeDataListBytes = nodeDataList.Count;

			// Assert
			var deltaNodeDataListBytes = postNodeDataListBytes - preNodeDataListBytes;
			deltaNodeDataListBytes.Should().Be(0);
		}
	}

	public class GetNode
	{
		[Fact]
		public void Should_get_added_node()
		{
			// Test Data
			var nodeData = new byte[] { 0, 1, 2, 3 };
			var hash = xxHash64.ComputeHash(nodeData);

			// Arrange
			var dataSource = new MemoryDataSource();
			dataSource.AddNode(nodeData.CreateCopy());

			// Act
			var actual = dataSource.GetNode(hash, new ToArrayReader());

			// Assert
			actual.Should().Equal(nodeData);
		}

		[Fact]
		public void Should_return_correct_data_when_multiple_nodes_exist()
		{
			// Test Data
			var nodeData1 = new byte[] { 0, 1, 2, 3 };
			var nodeData2 = new byte[] { 4, 5, 6, 7 };
			var nodeData3 = new byte[] { 8, 9, 10, 11 };
			var hash = xxHash64.ComputeHash(nodeData2);

			// Arrange
			var dataSource = new MemoryDataSource();
			dataSource.AddNode(nodeData1.CreateCopy());
			dataSource.AddNode(nodeData2.CreateCopy());
			dataSource.AddNode(nodeData3.CreateCopy());

			// Act
			var actual = dataSource.GetNode(hash, new ToArrayReader());

			// Assert
			actual.Should().Equal(nodeData2);
		}

		[Fact]
		public void Should_throw_if_called_with_nonexistent_hash()
		{
			// Arrange
			var dataSource = new MemoryDataSource();

			// Assert
			dataSource.Invoking(ts => ts.GetNode<object?>(0, new ToArrayReader()))
				.Should()
				.Throw<HashNotFoundException>();
		}
	}

	public class AddSnapshot
	{
		[Fact]
		public void Should_add_to_snapshot_index()
		{
			// Test Data
			ulong parentHash = 1;
			ulong rootNodeHash = 2;

			// Arrange
			var snapshotIndex = new Dictionary<ulong, SnapshotData>();
			var dataSource = new MemoryDataSource(snapshotIndex: snapshotIndex);

			// Act
			dataSource.AddSnapshot(parentHash, rootNodeHash);

			// Assert
			snapshotIndex.Count.Should().Be(1);
		}

		[Fact]
		public void Should_not_throw_on_duplicate_snapshot()
		{
			// Test Data
			ulong parentHash = 1;
			ulong rootNodeHash = 2;

			// Arrange
			var dataSource = new MemoryDataSource();

			// Assert
			dataSource.Invoking(source =>
					{
						source.AddSnapshot(parentHash, rootNodeHash);
						source.AddSnapshot(parentHash, rootNodeHash);
					}
				)
				.Should()
				.NotThrow();
		}
	}

	public class GetSnapshotParent
	{
		[Fact]
		public void Should_return_correct_parent_snapshot_hash()
		{
			// Test Data
			ulong parentHash = 1;
			ulong rootNodeHash = 2;

			// Arrange
			var dataSource = new MemoryDataSource();

			// Act
			var hash = dataSource.AddSnapshot(parentHash, rootNodeHash);

			// Assert
			dataSource.GetSnapshotParent(hash).Should().Be(1UL);
		}

		[Fact]
		public void Should_throw_if_GetSnapshotParent_called_with_nonexistent_hash()
		{
			// Arrange
			var dataSource = new MemoryDataSource();

			// Assert
			dataSource.Invoking(ts => ts.GetSnapshotParent(0))
				.Should()
				.Throw<HashNotFoundException>();
		}
	}

	public class GetSnapshotRootNode
	{
		[Fact]
		public void Should_return_correct_root_node_hash()
		{
			// Test Data
			ulong parentHash = 1;
			ulong rootNodeHash = 2;

			// Arrange
			var dataSource = new MemoryDataSource();

			// Act
			var hash = dataSource.AddSnapshot(parentHash, rootNodeHash);

			// Assert
			dataSource.GetSnapshotRootNode(hash).Should().Be(2UL);
		}

		[Fact]
		public void Should_throw_if_GetSnapshotRootNode_called_with_nonexistent_hash()
		{
			// Arrange
			var dataSource = new MemoryDataSource();

			// Assert
			dataSource.Invoking(ts => ts.GetSnapshotRootNode(0))
				.Should()
				.Throw<HashNotFoundException>();
		}
	}

	public class GetLeafSnapshotHashes
	{
		[Fact]
		public void Should_return_only_snapshot_hash()
		{
			// Arrange
			var dataSource = new MemoryDataSource();

			// Act
			var snapshotHash = dataSource.AddSnapshot(0, 2);

			// Assert
			dataSource.GetLeafSnapshotHashes().Should().BeEquivalentTo(new[] { snapshotHash });
		}

		[Fact]
		public void Should_return_the_latest_snapshot_hash_in_a_branch()
		{
			// Arrange
			var dataSource = new MemoryDataSource();

			// Act
			var rootHash = dataSource.AddSnapshot(0, 2);
			var childHash = dataSource.AddSnapshot(rootHash, 3);

			// Assert
			dataSource.GetLeafSnapshotHashes().Should().BeEquivalentTo(new[] { childHash });
		}

		[Fact]
		public void Should_return_the_latest_snapshot_hash_in_all_branches()
		{
			// Arrange
			var dataSource = new MemoryDataSource();

			// Act
			var rootHash = dataSource.AddSnapshot(0, 2);
			var childHash1 = dataSource.AddSnapshot(rootHash, 3);
			var childHash2 = dataSource.AddSnapshot(rootHash, 4);

			// Assert
			dataSource.GetLeafSnapshotHashes().Should().BeEquivalentTo(new[] { childHash1, childHash2 });
		}
	}

	public class ReconstitutionConstructor
	{
		[Fact]
		public void Should_initialize_data_source_with_node()
		{
			// Test Data
			var nodeHash = 123UL;
			var nodeIndex = ArrayX.Concat(ByteEncoder.GetBytes(nodeHash), new byte[8]);

			// Arrange/Act
			var nodeIndexStream = new MemoryStream(nodeIndex.CreateCopy());
			var dataSource = new MemoryDataSource(
				snapshotIndexSource: Stream.Null,
				leafSnapshotsSource: Stream.Null,
				nodeIndexSource: nodeIndexStream,
				nodeDataSource: Stream.Null
			);

			// Assert
			dataSource.HasNode(nodeHash).Should().BeTrue();
		}

		[Fact]
		public void Should_initialize_data_source_with_correct_node_data()
		{
			// Test Data
			var hash1 = 0123UL;
			var hash2 = 4567UL;
			var nodeData = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };
			var nodeIndexEntry = ArrayX.Concat(
				ByteEncoder.GetBytes(hash1),
				ByteEncoder.GetBytes(0),
				ByteEncoder.GetBytes(4),
				ByteEncoder.GetBytes(hash2),
				ByteEncoder.GetBytes(4),
				ByteEncoder.GetBytes(4)
			);

			// Arrange/Act
			var nodeIndexStream = new MemoryStream(nodeIndexEntry.CreateCopy());
			var nodeDataStream = new MemoryStream(nodeData.CreateCopy());
			var dataSource = new MemoryDataSource(
				snapshotIndexSource: Stream.Null,
				leafSnapshotsSource: Stream.Null,
				nodeIndexSource: nodeIndexStream,
				nodeDataSource: nodeDataStream
			);

			// Assert
			var result1 = dataSource.GetNode(hash1, new ToArrayReader());
			var result2 = dataSource.GetNode(hash2, new ToArrayReader());
			result1.Should().Equal(nodeData[..4]);
			result2.Should().Equal(nodeData[4..8]);
		}

		[Fact]
		public void Should_initialize_data_source_with_snapshot()
		{
			// Test Data
			var hash = 123UL;
			var snapshotIndex = ArrayX.Concat(ByteEncoder.GetBytes(hash), new byte[16]);

			// Arrange/Act
			var snapshotIndexStream = new MemoryStream(snapshotIndex);
			var dataSource = new MemoryDataSource(
				snapshotIndexSource: snapshotIndexStream,
				leafSnapshotsSource: Stream.Null,
				nodeIndexSource: Stream.Null,
				nodeDataSource: Stream.Null
			);

			// Assert
			dataSource.HasSnapshot(hash).Should().BeTrue();
		}

		[Fact]
		public void Should_initialize_data_source_with_correct_snapshot_data()
		{
			// Test Data
			var hash = 123UL;
			var parentHash = 5UL;
			var rootNodeHash = 42UL;
			var snapshotIndexEntry = ArrayX.Concat(
				ByteEncoder.GetBytes(hash),
				ByteEncoder.GetBytes(parentHash),
				ByteEncoder.GetBytes(rootNodeHash)
			);

			// Arrange/Act
			var snapshotIndexStream = new MemoryStream(snapshotIndexEntry.CreateCopy());
			var dataSource = new MemoryDataSource(
				snapshotIndexSource: snapshotIndexStream,
				leafSnapshotsSource: Stream.Null,
				nodeIndexSource: Stream.Null,
				nodeDataSource: Stream.Null
			);

			// Assert
			var actualParentHash = dataSource.GetSnapshotParent(hash);
			var actualRootNodeHash = dataSource.GetSnapshotRootNode(hash);
			actualParentHash.Should().Be(parentHash);
			actualRootNodeHash.Should().Be(rootNodeHash);
		}

		[Fact]
		public void Should_not_overallocate_node_data_list()
		{
			var nodeDataStream = new MemoryStream(8);
			nodeDataStream.Write(new byte[] { 1, 2, 3 });
			var nodeDataList = new SpannableList<byte>();
			var _ = new MemoryDataSource(
				snapshotIndexSource: Stream.Null,
				leafSnapshotsSource: Stream.Null,
				nodeIndexSource: Stream.Null,
				nodeDataSource: nodeDataStream,
				nodeData: nodeDataList
			);

			nodeDataList.Count.Should().Be(3);
		}
	}
}
