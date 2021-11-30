using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Pando.Exceptions;
using Pando.Repositories;
using Pando.Repositories.Utils;
using PandoTests.Utils;
using Standart.Hash.xxHash;

namespace PandoTests.Repositories.InMemoryRepositoryTests
{
	public class NodeOperations
	{
		[Test]
		public void Should_add_node()
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

		[Test]
		public void Should_get_added_node()
		{
			// Test Data
			var nodeData = new byte[] { 0, 1, 2, 3 };
			var hash = xxHash64.ComputeHash(nodeData);

			// Arrange
			var repository = new InMemoryRepository();
			repository.AddNode(nodeData.CreateCopy());

			// Act
			var actual = repository.GetNode(hash, bytes => bytes.ToArray());

			// Assert
			actual.Should().Equal(nodeData);
		}

		[Test]
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

		[Test]
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

		[Test]
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
			var actual = repository.GetNode(hash, bytes => bytes.ToArray());

			// Assert
			actual.Should().Equal(nodeData2);
		}

		[Test]
		public void Should_throw_if_GetNodeData_called_with_nonexistent_hash()
		{
			// Arrange
			var repository = new InMemoryRepository();

			// Assert
			repository.Invoking(ts => ts.GetNode<object?>(0, _ => null))
				.Should()
				.Throw<HashNotFoundException>();
		}
	}
}
