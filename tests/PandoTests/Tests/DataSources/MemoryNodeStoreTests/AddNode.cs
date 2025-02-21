using System;
using System.Collections.Generic;
using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Repositories;
using PandoTests.Utils;
using Xunit;

namespace PandoTests.Tests.DataSources.MemoryNodeStoreTests;

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
		var dataSource = new MemoryNodeStore(
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
		var dataSource = new MemoryNodeStore();

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
		var dataSource = new MemoryNodeStore(nodeData: nodeDataList);

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
