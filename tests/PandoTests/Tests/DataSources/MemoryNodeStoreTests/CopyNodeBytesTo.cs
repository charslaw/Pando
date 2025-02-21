using System;
using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Exceptions;
using Pando.Repositories;
using PandoTests.Utils;
using Xunit;

namespace PandoTests.Tests.DataSources.MemoryNodeStoreTests;

public class CopyNodeBytesTo
{
	[Fact]
	public void Should_get_added_node()
	{
		// Test Data
		var nodeData = new byte[] { 0, 1, 2, 3 };
		var nodeId = HashUtils.ComputeNodeHash(nodeData);

		// Arrange
		var dataSource = new MemoryNodeStore();
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
		var dataSource = new MemoryNodeStore();
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
		var dataSource = new MemoryNodeStore();

		// Assert
		dataSource.Invoking(source =>
				{
					Span<byte> buffer = stackalloc byte[0];
					source.CopyNodeBytesTo(NodeId.None, buffer);
				}
			)
			.Should()
			.Throw<NodeIdNotFoundException>();
	}
}
