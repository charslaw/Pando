using FluentAssertions;
using Pando.Exceptions;
using Pando.Repositories;
using Pando.Serialization.Primitives;
using Xunit;

namespace PandoTests.Tests.Repositories.PandoRepositoryTests;

public class MergeSnapshots
{
	[Fact]
	public void Should_throw_if_source_snapshot_is_ancestor_of_target_snapshot()
	{
		var repository = new PandoRepository<int>(new Int32LittleEndianSerializer());

		var root = repository.SaveRootSnapshot(0);
		var child = repository.SaveSnapshot(1, root);

		repository.Invoking(x => x.MergeSnapshots(root, child))
			.Should().Throw<InvalidMergeException>();
	}

	[Fact]
	public void Should_throw_if_target_snapshot_is_ancestor_of_source_snapshot()
	{
		var repository = new PandoRepository<int>(new Int32LittleEndianSerializer());
		var root = repository.SaveRootSnapshot(0);
		var child = repository.SaveSnapshot(1, root);

		repository.Invoking(x => x.MergeSnapshots(child, root))
			.Should().Throw<InvalidMergeException>();
	}

	[Fact]
	public void Should_throw_if_source_and_target_are_same()
	{
		var repository = new PandoRepository<int>(new Int32LittleEndianSerializer());
		var root = repository.SaveRootSnapshot(0);

		repository.Invoking(x => x.MergeSnapshots(root, root))
			.Should().Throw<InvalidMergeException>();
	}
}
