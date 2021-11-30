using System.IO;
using FluentAssertions;
using NUnit.Framework;
using Pando.Repositories;
using PandoTests.Utils;

namespace PandoTests.Repositories
{
	public class PersistenceBackedRepositoryTests
	{
		[Test]
		public void Should_not_output_duplicate_node_to_stream_repository()
		{
			// Test Data
			var nodeData = new byte[] { 1, 2, 3, 4 };
			var expectedIndexLength = sizeof(ulong) + sizeof(int) + sizeof(int);
			var expectedDataLength = nodeData.Length;

			// Arrange
			var nodeIndexStream = new MemoryStream();
			var nodeDataStream = new MemoryStream();
			var streamRepo = new StreamRepository(Stream.Null, nodeIndexStream, nodeDataStream);
			var inMemoryRepo = new InMemoryRepository();
			var repository = new PersistenceBackedRepository(inMemoryRepo, streamRepo);

			// Act
			repository.AddNode(nodeData.CreateCopy());
			repository.AddNode(nodeData.CreateCopy());

			// Assert
			var actualIndex = nodeIndexStream.ToArray();
			var actualData = nodeDataStream.ToArray();

			actualIndex.Length.Should().Be(expectedIndexLength);
			actualData.Length.Should().Be(expectedDataLength);
		}

		[Test]
		public void Should_not_output_duplicate_snapshot_to_stream_repository()
		{
			// Test Data
			var parentHash = 5UL;
			var rootNodeHash = 27UL;
			var expectedIndexLength = sizeof(ulong) * 3;

			// Arrange
			var snapshotIndexStream = new MemoryStream();
			var streamRepo = new StreamRepository(snapshotIndexStream, Stream.Null, Stream.Null);
			var inMemoryRepo = new InMemoryRepository();
			var repository = new PersistenceBackedRepository(inMemoryRepo, streamRepo);

			// Act
			repository.AddSnapshot(parentHash, rootNodeHash);
			repository.AddSnapshot(parentHash, rootNodeHash);

			// Assert
			var actualIndex = snapshotIndexStream.ToArray();

			actualIndex.Length.Should().Be(expectedIndexLength);
		}
	}
}
