using System.Collections.Immutable;

namespace Pando;

public record SnapshotChain<T>
{
	public readonly ImmutableArray<SnapshotChain<T>> Children;
	private readonly ulong _snapshotHash;
	private readonly IPandoSaver<T> _saver;

	internal SnapshotChain(ulong snapshotHash, ImmutableArray<SnapshotChain<T>> children, IPandoSaver<T> saver)
	{
		Children = children;
		_snapshotHash = snapshotHash;
		_saver = saver;
	}

	public T GetTreeRoot() => _saver.GetSnapshot(_snapshotHash);
}