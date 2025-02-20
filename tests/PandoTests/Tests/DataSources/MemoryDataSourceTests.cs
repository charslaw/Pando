using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
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
			var nodeIndex = new Dictionary<NodeId, Range>();
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

	public class CopyNodeBytesTo
	{
		[Fact]
		public void Should_get_added_node()
		{
			// Test Data
			var nodeData = new byte[] { 0, 1, 2, 3 };
			var nodeId = HashUtils.ComputeNodeHash(nodeData);

			// Arrange
			var dataSource = new MemoryDataSource();
			dataSource.AddNode(nodeData.CreateCopy());

			// Act
			var actual = new byte[4];
			dataSource.CopyNodeBytesTo(nodeId, actual);

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
			var node2Id = HashUtils.ComputeNodeHash(nodeData2);

			// Arrange
			var dataSource = new MemoryDataSource();
			dataSource.AddNode(nodeData1.CreateCopy());
			dataSource.AddNode(nodeData2.CreateCopy());
			dataSource.AddNode(nodeData3.CreateCopy());

			// Act
			var actual = new byte[4];
			dataSource.CopyNodeBytesTo(node2Id, actual);

			// Assert
			actual.Should().Equal(nodeData2);
		}

		[Fact]
		public void Should_throw_if_called_with_nonexistent_hash()
		{
			// Arrange
			var dataSource = new MemoryDataSource();

			// Assert
			dataSource.Invoking(source =>
					{
						Span<byte> buffer = stackalloc byte[0];
						source.CopyNodeBytesTo(NodeId.None, buffer);
					}
				)
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
			var parentSnapshotId = new SnapshotId(1);
			var rootNodeId = new NodeId(2);

			// Arrange
			var snapshotIndex = new Dictionary<SnapshotId, SnapshotData>();
			var dataSource = new MemoryDataSource(snapshotIndex: snapshotIndex);

			// Act
			dataSource.AddSnapshot(parentSnapshotId, rootNodeId);

			// Assert
			snapshotIndex.Count.Should().Be(1);
		}

		[Fact]
		public void Should_not_throw_on_duplicate_snapshot()
		{
			// Test Data
			var parentSnapshotId = new SnapshotId(1);
			var rootNodeId = new NodeId(2);

			// Arrange
			var dataSource = new MemoryDataSource();

			// Assert
			dataSource.Invoking(source =>
					{
						source.AddSnapshot(parentSnapshotId, rootNodeId);
						source.AddSnapshot(parentSnapshotId, rootNodeId);
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
			var parentSnapshotId = new SnapshotId(1);
			var rootNodeId = new NodeId(2);

			// Arrange
			var dataSource = new MemoryDataSource();

			// Act
			var snapshotId = dataSource.AddSnapshot(parentSnapshotId, rootNodeId);

			// Assert
			dataSource.GetSnapshotParent(snapshotId).Should().Be(new SnapshotId(1));
		}

		[Fact]
		public void Should_throw_if_GetSnapshotParent_called_with_nonexistent_hash()
		{
			// Arrange
			var dataSource = new MemoryDataSource();

			// Assert
			dataSource.Invoking(ts => ts.GetSnapshotParent(SnapshotId.None))
				.Should()
				.Throw<HashNotFoundException>();
		}
	}

	public class GetSnapshotLeastCommonAncestor
	{
		[Fact]
		public void Should_return_correct_least_common_ancestor_snapshot_hash()
		{
			var dataSource = new MemoryDataSource();

			var rootId = dataSource.AddSnapshot(new SnapshotId(0), new NodeId(1));
			var childId = dataSource.AddSnapshot(rootId, new NodeId(1));
			var branch1Id = dataSource.AddSnapshot(childId, new NodeId(1));
			var branch2AId = dataSource.AddSnapshot(childId, new NodeId(2));
			var branch2BId = dataSource.AddSnapshot(branch2AId, new NodeId(1));

			var lca = dataSource.GetSnapshotLeastCommonAncestor(branch1Id, branch2BId);

			lca.Should().Be(childId);
		}
	}

	public class GetSnapshotRootNode
	{
		[Fact]
		public void Should_return_correct_root_node_hash()
		{
			// Test Data
			var parentSnapshotId = new SnapshotId(1);
			var rootNodeId = new NodeId(2);

			// Arrange
			var dataSource = new MemoryDataSource();

			// Act
			var snapshotId = dataSource.AddSnapshot(parentSnapshotId, rootNodeId);

			// Assert
			dataSource.GetSnapshotRootNode(snapshotId).Should().Be(new NodeId(2));
		}

		[Fact]
		public void Should_throw_if_GetSnapshotRootNode_called_with_nonexistent_hash()
		{
			// Arrange
			var dataSource = new MemoryDataSource();

			// Assert
			dataSource.Invoking(ts => ts.GetSnapshotRootNode(SnapshotId.None))
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
			var snapshotHash = dataSource.AddSnapshot(SnapshotId.None, new NodeId(2));

			// Assert
			dataSource.GetLeafSnapshotIds().Should().BeEquivalentTo(new[] { snapshotHash });
		}

		[Fact]
		public void Should_return_the_latest_snapshot_hash_in_a_branch()
		{
			// Arrange
			var dataSource = new MemoryDataSource();

			// Act
			var rootId = dataSource.AddSnapshot(SnapshotId.None, new NodeId(2));
			var childId = dataSource.AddSnapshot(rootId, new NodeId(3));

			// Assert
			dataSource.GetLeafSnapshotIds().Should().BeEquivalentTo(new[] { childId });
		}

		[Fact]
		public void Should_return_the_latest_snapshot_hash_in_all_branches()
		{
			// Arrange
			var dataSource = new MemoryDataSource();

			// Act
			var rootId = dataSource.AddSnapshot(SnapshotId.None, new NodeId(2));
			var child1Id = dataSource.AddSnapshot(rootId, new NodeId(3));
			var child2Id = dataSource.AddSnapshot(rootId, new NodeId(4));

			// Assert
			dataSource.GetLeafSnapshotIds().Should().BeEquivalentTo(new[] { child1Id, child2Id });
		}
	}

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
				leafSnapshotsSource: Stream.Null,
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
				leafSnapshotsSource: Stream.Null,
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
				leafSnapshotsSource: Stream.Null,
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
				leafSnapshotsSource: Stream.Null,
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
				leafSnapshotsSource: Stream.Null,
				nodeIndexSource: Stream.Null,
				nodeDataSource: nodeDataStream,
				nodeData: nodeDataList
			);

			nodeDataList.Count.Should().Be(3);
		}
	}
}
