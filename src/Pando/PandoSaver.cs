using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Pando.Exceptions;
using Pando.Repositories;
using Pando.Repositories.Utils;

namespace Pando
{
	public class PandoSaver<T> : IPandoSaver<T>
	{
		private readonly IPandoRepository _repository;
		private readonly IPandoNodeSerializer<T> _serializer;

		// preallocate delegate to avoid making an allocation every time we deserialize a node.
		private readonly SpanVisitor<byte, T> _deserializeDelegate;
		private T Deserialize(ReadOnlySpan<byte> bytes) => _serializer.Deserialize(bytes, _repository);

		private ulong _currentSnapshot;

		public PandoSaver(IPandoRepository repository, IPandoNodeSerializer<T> serializer)
		{
			_repository = repository;
			_serializer = serializer;
			_deserializeDelegate = Deserialize;
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
			return _repository.GetNode(nodeHash, _deserializeDelegate);
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
}
