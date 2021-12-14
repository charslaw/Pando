using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Pando.Exceptions;
using Pando.Repositories;
using Pando.Repositories.Utils;
using PandoTests.Utils;
using Standart.Hash.xxHash;
using Xunit;

namespace PandoTests.Tests.Repositories;

public class InMemoryRepositoryTests
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
			var repository = new InMemoryRepository(
				nodeIndex: nodeIndex,
				nodeData: nodeDataList
			);

			// Act
			repository.AddNode(nodeData.CreateCopy());

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
			var repository = new InMemoryRepository();

			// Assert
			repository.Invoking(repo =>
					{
						repo.AddNode(nodeData.CreateCopy());
						repo.AddNode(nodeData.CreateCopy());
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
			var repository = new InMemoryRepository(nodeData: nodeDataList);

			// Act
			repository.AddNode(nodeData.CreateCopy());
			var preNodeDataListBytes = nodeDataList.Count;
			repository.AddNode(nodeData.CreateCopy());
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
			var repository = new InMemoryRepository();
			repository.AddNode(nodeData.CreateCopy());

			// Act
			var actual = repository.GetNode(hash, new ToArrayDeserializer());

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
			var repository = new InMemoryRepository();
			repository.AddNode(nodeData1.CreateCopy());
			repository.AddNode(nodeData2.CreateCopy());
			repository.AddNode(nodeData3.CreateCopy());

			// Act
			var actual = repository.GetNode(hash, new ToArrayDeserializer());

			// Assert
			actual.Should().Equal(nodeData2);
		}

		[Fact]
		public void Should_throw_if_called_with_nonexistent_hash()
		{
			// Arrange
			var repository = new InMemoryRepository();

			// Assert
			repository.Invoking(ts => ts.GetNode<object?>(0, new ToArrayDeserializer()))
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
			var repository = new InMemoryRepository(snapshotIndex: snapshotIndex);

			// Act
			repository.AddSnapshot(parentHash, rootNodeHash);

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
			var repository = new InMemoryRepository();

			// Assert
			repository.Invoking(repo =>
					{
						repo.AddSnapshot(parentHash, rootNodeHash);
						repo.AddSnapshot(parentHash, rootNodeHash);
					}
				)
				.Should()
				.NotThrow();
		}

		[Fact]
		public void Should_set_latest_snapshot()
		{
			// Arrange
			var repository = new InMemoryRepository();
			var hash1 = repository.AddSnapshot(0, 1UL);
			var hash2 = repository.AddSnapshot(hash1, 2UL);

			// Assert
			repository.LatestSnapshot.Should().Be(hash2);
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
			var repository = new InMemoryRepository();

			// Act
			var hash = repository.AddSnapshot(parentHash, rootNodeHash);

			// Assert
			repository.GetSnapshotParent(hash).Should().Be(1UL);
		}

		[Fact]
		public void Should_throw_if_GetSnapshotParent_called_with_nonexistent_hash()
		{
			// Arrange
			var repository = new InMemoryRepository();

			// Assert
			repository.Invoking(ts => ts.GetSnapshotParent(0))
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
			var repository = new InMemoryRepository();

			// Act
			var hash = repository.AddSnapshot(parentHash, rootNodeHash);

			// Assert
			repository.GetSnapshotRootNode(hash).Should().Be(2UL);
		}

		[Fact]
		public void Should_throw_if_GetSnapshotRootNode_called_with_nonexistent_hash()
		{
			// Arrange
			var repository = new InMemoryRepository();

			// Assert
			repository.Invoking(ts => ts.GetSnapshotRootNode(0))
				.Should()
				.Throw<HashNotFoundException>();
		}
	}

	public class LatestSnapshot
	{
		[Fact]
		public void Should_be_zero_before_any_snapshots_have_been_added()
		{
			var repository = new InMemoryRepository();

			repository.LatestSnapshot.Should().Be(0);
		}
	}

	public class GetAllSnapshotHashes
	{
		[Fact]
		public void Should_get_all_snapshot_hashes()
		{
			// Arrange
			var repository = new InMemoryRepository();
			var hash1 = repository.AddSnapshot(1UL, 2UL);
			var hash2 = repository.AddSnapshot(3UL, 5UL);
			var hash3 = repository.AddSnapshot(8UL, 13UL);

			// Act
			var actual = repository.GetAllSnapshotEntries();

			// Assert
			var expected = new SnapshotEntry[]
			{
				new(hash1, 1UL, 2U),
				new(hash2, 3UL, 5UL),
				new(hash3, 8UL, 13UL),
			};
			actual.Should().Equal(expected);
		}
	}

	public class ReconstitutionConstructor
	{
		[Fact]
		public void Should_initialize_repository_with_node()
		{
			// Test Data
			var nodeHash = 123UL;
			var nodeIndex = ArrayX.Concat(ByteEncoder.GetBytes(nodeHash), new byte[8]);

			// Arrange/Act
			var nodeIndexStream = new MemoryStream(nodeIndex.CreateCopy());
			var repository = new InMemoryRepository(Stream.Null, nodeIndexStream, Stream.Null);

			// Assert
			repository.HasNode(nodeHash).Should().BeTrue();
		}

		[Fact]
		public void Should_initialize_repository_with_correct_node_data()
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
			var repository = new InMemoryRepository(Stream.Null, nodeIndexStream, nodeDataStream);

			// Assert
			var result1 = repository.GetNode(hash1, new ToArrayDeserializer());
			var result2 = repository.GetNode(hash2, new ToArrayDeserializer());
			result1.Should().Equal(nodeData[..4]);
			result2.Should().Equal(nodeData[4..8]);
		}

		[Fact]
		public void Should_initialize_repository_with_snapshot()
		{
			// Test Data
			var hash = 123UL;
			var snapshotIndex = ArrayX.Concat(ByteEncoder.GetBytes(hash), new byte[16]);

			// Arrange/Act
			var snapshotIndexStream = new MemoryStream(snapshotIndex);
			var repository = new InMemoryRepository(snapshotIndexStream, Stream.Null, Stream.Null);

			// Assert
			repository.HasSnapshot(hash).Should().BeTrue();
		}

		[Fact]
		public void Should_initialize_repository_with_correct_snapshot_data()
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
			var repository = new InMemoryRepository(snapshotIndexStream, Stream.Null, Stream.Null);

			// Assert
			var actualParentHash = repository.GetSnapshotParent(hash);
			var actualRootNodeHash = repository.GetSnapshotRootNode(hash);
			actualParentHash.Should().Be(parentHash);
			actualRootNodeHash.Should().Be(rootNodeHash);
		}

		[Fact]
		public void Should_not_overallocate_node_data_list()
		{
			var nodeDataStream = new MemoryStream(8);
			nodeDataStream.Write(new byte[] { 1, 2, 3 });
			var nodeDataList = new SpannableList<byte>();
			var _ = new InMemoryRepository(
				Stream.Null, Stream.Null, nodeDataStream,
				nodeData: nodeDataList
			);

			nodeDataList.Count.Should().Be(3);
		}

		[Fact]
		public void Should_initialize_LatestSnapshot()
		{
			var hash1 = 1UL;
			var hash2 = 2UL;
			var snapshotIndex = ArrayX.Concat(
				ByteEncoder.GetBytes(hash1),
				new byte[16],
				ByteEncoder.GetBytes(hash2),
				new byte[16]
			);

			var snapshotIndexStream = new MemoryStream(snapshotIndex.CreateCopy());
			var repository = new InMemoryRepository(snapshotIndexStream, Stream.Null, Stream.Null);

			repository.LatestSnapshot.Should().Be(hash2);
		}
	}
}
