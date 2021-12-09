using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Pando.Exceptions;
using Pando.Repositories;

namespace Pando;

public class PandoSaver<T> : IPandoSaver<T>
{
	private readonly IPandoRepository _repository;
	private readonly IPandoNodeSerializerDeserializer<T> _serializer;

	private ulong _currentSnapshot;

	public PandoSaver(IPandoRepository repository, IPandoNodeSerializerDeserializer<T> serializer)
	{
		_repository = repository;
		_serializer = serializer;
		_currentSnapshot = repository.LatestSnapshot;
	}

	public ulong SaveSnapshot(T tree)
	{
		var nodeHash = _serializer.Serialize(tree, _repository);
		_currentSnapshot = _repository.AddSnapshot(_currentSnapshot, nodeHash);
		return _currentSnapshot;
	}

	public T GetSnapshot(ulong hash)
	{
		var nodeHash = _repository.GetSnapshotRootNode(hash);
		return _repository.GetNode(nodeHash, _serializer);
	}

	public SnapshotChain<T> GetFullSnapshotChain()
	{
		var snapshotEntries = _repository.GetAllSnapshotEntries().ToArray();

		foreach (var entry in snapshotEntries)
		{
			if (entry.ParentHash == 0) return BuildSnapshotLink(entry, snapshotEntries);
		}

		throw new NoRootSnapshotException();
	}

	private SnapshotChain<T> BuildSnapshotLink(SnapshotEntry thisSnapshot, SnapshotEntry[] entries)
	{
		var (hash, _, _) = thisSnapshot;

		var children = new List<SnapshotChain<T>>();
		// ReSharper disable once LoopCanBeConvertedToQuery
		foreach (var entry in entries)
		{
			if (entry.ParentHash != hash) continue;

			children.Add(BuildSnapshotLink(entry, entries));
		}

		return new SnapshotChain<T>(hash, children.ToImmutableArray(), this);
	}
}
