using FluentAssertions;
using Pando.Exceptions;
using Pando.Repositories;
using Xunit;

namespace PandoTests.Repositories.InMemoryRepositoryTests;

public class SnapshotOperations
{
	[Fact]
	public void Should_add_snapshot()
	{
		// Test Data
		ulong parentHash = 1;
		ulong rootNodeHash = 2;

		// Arrange
		var repository = new InMemoryRepository();

		// Act
		var hash = repository.AddSnapshot(parentHash, rootNodeHash);

		// Assert
		var actualParent = repository.GetSnapshotParent(hash);
		var actualRootNode = repository.GetSnapshotRootNode(hash);
		actualParent.Should().Be(parentHash);
		actualRootNode.Should().Be(rootNodeHash);
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
	public void Should_throw_if_GetSnapshotParent_called_with_nonexistent_hash()
	{
		// Arrange
		var repository = new InMemoryRepository();

		// Assert
		repository.Invoking(ts => ts.GetSnapshotParent(0))
			.Should()
			.Throw<HashNotFoundException>();
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

	[Fact]
	public void Should_have_zero_latest_snapshot_before_any_snapshots_have_been_added()
	{
		var repository = new InMemoryRepository();

		repository.LatestSnapshot.Should().Be(0);
	}

	[Fact]
	public void Should_set_latest_snapshot_when_adding_snapshot()
	{
		// Arrange
		var repository = new InMemoryRepository();
		var hash1 = repository.AddSnapshot(0, 1UL);
		var hash2 = repository.AddSnapshot(hash1, 2UL);

		// Assert
		repository.LatestSnapshot.Should().Be(hash2);
	}

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
