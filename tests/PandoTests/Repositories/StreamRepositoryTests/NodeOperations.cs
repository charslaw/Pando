using System.IO;
using FluentAssertions;
using MiscUtil.Conversion;
using NUnit.Framework;
using Pando.Repositories;
using PandoTests.Utils;
using Standart.Hash.xxHash;

namespace PandoTests.Repositories.StreamRepositoryTests
{
	public class NodeOperations
	{
		private static readonly EndianBitConverter bitConverter = EndianBitConverter.Little;

		[Test]
		[TestCase(new byte[] { 1 })]
		[TestCase(new byte[] { 1, 2, 3 })]
		[TestCase(new byte[] { 1, 2, 3, 4 })]
		public void Should_output_node_index_when_node_added(byte[] nodeData)
		{
			// Test Data
			ulong hash = xxHash64.ComputeHash(nodeData);
			var expected = ArrayX.Concat(
				bitConverter.GetBytes(hash),
				bitConverter.GetBytes(0),
				bitConverter.GetBytes(nodeData.Length)
			);

			// Arrange
			var nodeIndexStream = new MemoryStream();
			using var repository = new StreamRepository(Stream.Null, nodeIndexStream, Stream.Null);

			// Act
			repository.AddNode(nodeData.CreateCopy());

			// Assert
			var nodeIndex = nodeIndexStream.ToArray();
			nodeIndex.Should().Equal(expected);
		}

		[Test]
		public void Should_output_node_index_sequentially()
		{
			// Test Data
			var nodeData = new[]
			{
				new byte[] { 1, 2, 3, 4 },
				new byte[] { 5, 6, 7 },
				new byte[] { 8, 9, 10, 11, 12 }
			};
			var expected = ArrayX.Concat(
				bitConverter.GetBytes(xxHash64.ComputeHash(nodeData[0], nodeData[0].Length)), // Node 0 hash           8 bytes    [0 ]
				bitConverter.GetBytes(0),                                                     // Node 0 data start     4 bytes    [8 ]
				bitConverter.GetBytes(nodeData[0].Length),                                    // Node 0 data length    4 bytes    [12]
				bitConverter.GetBytes(xxHash64.ComputeHash(nodeData[1], nodeData[1].Length)), // Node 1 hash           8 bytes    [16]
				bitConverter.GetBytes(nodeData[0].Length),                                    // Node 1 data start     4 bytes    [24]
				bitConverter.GetBytes(nodeData[1].Length),                                    // Node 1 data length    4 bytes    [28]
				bitConverter.GetBytes(xxHash64.ComputeHash(nodeData[2], nodeData[2].Length)), // Node 2 hash           8 bytes    [32]
				bitConverter.GetBytes(nodeData[0].Length + nodeData[1].Length),               // Node 2 data start     4 bytes    [40]
				bitConverter.GetBytes(nodeData[2].Length)                                     // Node 2 data length    4 bytes    [44]
			);                                                                                // Total                 48 bytes

			// Arrange
			var nodeIndexStream = new MemoryStream();
			using var repository = new StreamRepository(Stream.Null, nodeIndexStream, Stream.Null);

			// Act
			repository.AddNode(nodeData[0]);
			repository.AddNode(nodeData[1]);
			repository.AddNode(nodeData[2]);

			// Assert
			var nodeIndex = nodeIndexStream.ToArray();

			nodeIndex.Should().Equal(expected);
		}

		[Test]
		public void Should_output_node_data_when_node_added()
		{
			// Test Data
			var nodeData = new byte[] { 1, 2, 3 };

			// Arrange
			var nodeDataStream = new MemoryStream();
			using var repository = new StreamRepository(Stream.Null, Stream.Null, nodeDataStream);

			// Act
			repository.AddNode(nodeData.CreateCopy());

			// Assert
			var allNodeData = nodeDataStream.ToArray();
			allNodeData.Should().Equal(nodeData);
		}

		[Test]
		public void Should_append_node_data_sequentially()
		{
			// Test Data
			var nodeData1 = new byte[] { 1, 2, 3, 4 };
			var nodeData2 = new byte[] { 5, 6, 7 };
			var nodeData3 = new byte[] { 8, 9, 10, 11, 12 };
			var expected = ArrayX.Concat(nodeData1, nodeData2, nodeData3);

			// Arrange
			var nodeDataStream = new MemoryStream();
			using var repository = new StreamRepository(Stream.Null, Stream.Null, nodeDataStream);

			// Act
			repository.AddNode(nodeData1);
			repository.AddNode(nodeData2);
			repository.AddNode(nodeData3);

			// Assert
			var allNodeData = nodeDataStream.ToArray();
			allNodeData.Should().Equal(expected);
		}
	}
}
