using System;
using System.Collections.Immutable;
using Pando.DataSources.Utils;

namespace Pando.DataSources;

public class PersistenceBackedDataSource : IDataSource, IDisposable
{
	private readonly MemoryDataSource _mainDataSource;
	private readonly StreamDataSource _persistentDataSource;

	public PersistenceBackedDataSource(MemoryDataSource mainDataSource, StreamDataSource persistentDataSource)
	{
		_mainDataSource = mainDataSource;
		_persistentDataSource = persistentDataSource;
	}

	public ulong AddNode(ReadOnlySpan<byte> bytes)
	{
		var hash = HashUtils.ComputeNodeHash(bytes);

		if (_mainDataSource.HasNode(hash)) return hash;

		_mainDataSource.AddNodeWithHashUnsafe(hash, bytes);
		_persistentDataSource.AddNodeWithHashUnsafe(hash, bytes);
		return hash;
	}

	public ulong AddSnapshot(ulong parentHash, ulong rootNodeHash)
	{
		var hash = HashUtils.ComputeSnapshotHash(parentHash, rootNodeHash);

		if (_mainDataSource.HasSnapshot(hash)) return hash;

		_mainDataSource.AddSnapshotWithHashUnsafe(hash, parentHash, rootNodeHash);
		_persistentDataSource.AddSnapshotWithHashUnsafe(hash, parentHash, rootNodeHash);
		return hash;
	}

	public bool HasNode(ulong hash) => _mainDataSource.HasNode(hash);
	public int GetSizeOfNode(ulong hash) => _mainDataSource.GetSizeOfNode(hash);
	public void CopyNodeBytesTo(ulong hash, ref Span<byte> outputBytes) => _mainDataSource.CopyNodeBytesTo(hash, ref outputBytes);

	public bool HasSnapshot(ulong hash) => _mainDataSource.HasSnapshot(hash);
	public int SnapshotCount => _mainDataSource.SnapshotCount;
	public ulong GetSnapshotParent(ulong hash) => _mainDataSource.GetSnapshotParent(hash);
	public ulong GetSnapshotRootNode(ulong hash) => _mainDataSource.GetSnapshotRootNode(hash);
	public IImmutableSet<ulong> GetLeafSnapshotHashes() => _mainDataSource.GetLeafSnapshotHashes();

	public void Dispose() => _persistentDataSource.Dispose();
}
