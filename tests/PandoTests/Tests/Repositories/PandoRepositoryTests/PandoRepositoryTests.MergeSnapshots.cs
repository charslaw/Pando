using Pando.Exceptions;
using Pando.Repositories;
using Pando.Serialization.Primitives;

namespace PandoTests.Tests.Repositories.PandoRepositoryTests;

public static partial class PandoRepositoryTests
{
	public class MergeSnapshots
	{
		[Test]
		public async Task Should_throw_if_source_snapshot_is_ancestor_of_target_snapshot()
		{
			var repository = new PandoRepository<int>(new Int32LittleEndianSerializer());

			var root = repository.SaveRootSnapshot(0);
			var child = repository.SaveSnapshot(1, root);

			await Assert.That(() => repository.MergeSnapshots(root, child)).ThrowsExactly<InvalidMergeException>();
		}

		[Test]
		public async Task Should_throw_if_target_snapshot_is_ancestor_of_source_snapshot()
		{
			var repository = new PandoRepository<int>(new Int32LittleEndianSerializer());
			var root = repository.SaveRootSnapshot(0);
			var child = repository.SaveSnapshot(1, root);

			await Assert.That(() => repository.MergeSnapshots(child, root)).ThrowsExactly<InvalidMergeException>();
		}

		[Test]
		public async Task Should_throw_if_source_and_target_are_same()
		{
			var repository = new PandoRepository<int>(new Int32LittleEndianSerializer());
			var root = repository.SaveRootSnapshot(0);

			await Assert.That(() => repository.MergeSnapshots(root, root)).ThrowsExactly<InvalidMergeException>();
		}
	}
}
