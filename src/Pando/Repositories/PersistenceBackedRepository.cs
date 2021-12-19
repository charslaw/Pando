using System;
using System.Collections.Generic;
using Pando.Repositories.Utils;

namespace Pando.Repositories;

public class PersistenceBackedRepository : IPandoRepository, IDisposable
{
	private readonly InMemoryRepository _mainRepository;
	private readonly StreamRepository _persistentRepository;

	public PersistenceBackedRepository(InMemoryRepository mainRepository, StreamRepository persistentRepository)
	{
		_mainRepository = mainRepository;
		_persistentRepository = persistentRepository;
	}

	public ulong AddNode(ReadOnlySpan<byte> bytes)
	{
		var hash = HashUtils.ComputeNodeHash(bytes);

		if (_mainRepository.HasNode(hash)) return hash;

		_mainRepository.AddNodeWithHashUnsafe(hash, bytes);
		_persistentRepository.AddNodeWithHashUnsafe(hash, bytes);
		return hash;
	}

	public ulong AddSnapshot(ulong parentHash, ulong rootNodeHash)
	{
		var hash = HashUtils.ComputeSnapshotHash(parentHash, rootNodeHash);

		if (_mainRepository.HasSnapshot(hash)) return hash;

		_mainRepository.AddSnapshotWithHashUnsafe(hash, parentHash, rootNodeHash);
		_persistentRepository.AddSnapshotWithHashUnsafe(hash, parentHash, rootNodeHash);
		return hash;
	}

	public bool HasNode(ulong hash) => _mainRepository.HasNode(hash);
	public bool HasSnapshot(ulong hash) => _mainRepository.HasSnapshot(hash);

	public T GetNode<T>(ulong hash, in IPandoNodeDeserializer<T> nodeDeserializer) => _mainRepository.GetNode(hash, in nodeDeserializer);
	public int GetSizeOfNode(ulong hash) => _mainRepository.GetSizeOfNode(hash);

	public ulong GetSnapshotParent(ulong hash) => _mainRepository.GetSnapshotParent(hash);
	public ulong GetSnapshotRootNode(ulong hash) => _mainRepository.GetSnapshotRootNode(hash);
	public IEnumerable<SnapshotEntry> GetAllSnapshotEntries() => _mainRepository.GetAllSnapshotEntries();

	public void Dispose() => _persistentRepository.Dispose();
}
