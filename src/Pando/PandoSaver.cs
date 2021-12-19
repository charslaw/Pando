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

	public PandoSaver(IPandoRepository repository, IPandoNodeSerializerDeserializer<T> serializer)
	{
		_repository = repository;
		_serializer = serializer;
	}

	public ulong SaveRootSnapshot(T tree)
	{
		if (_repository.HasAnySnapshot()) throw new AlreadyHasRootSnapshotException();

		var nodeHash = _serializer.Serialize(tree, _repository);
		return _repository.AddSnapshot(0UL, nodeHash);
	}

	public ulong SaveSnapshot(T tree, ulong parentHash)
	{
		var nodeHash = _serializer.Serialize(tree, _repository);
		return _repository.AddSnapshot(parentHash, nodeHash);
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
