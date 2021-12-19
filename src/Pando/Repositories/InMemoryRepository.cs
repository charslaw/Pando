using System;
using System.Collections.Generic;
using System.IO;
using Pando.Exceptions;
using Pando.Repositories.Utils;

namespace Pando.Repositories;

public class InMemoryRepository : IPandoRepository
{
	private readonly Dictionary<ulong, SnapshotData> _snapshotIndex = new();
	private readonly Dictionary<ulong, DataSlice> _nodeIndex = new();
	private readonly SpannableList<byte> _nodeData = new();

	public InMemoryRepository() { }

	internal InMemoryRepository(
		Dictionary<ulong, SnapshotData>? snapshotIndex = null,
		Dictionary<ulong, DataSlice>? nodeIndex = null,
		SpannableList<byte>? nodeData = null
	)
	{
		_snapshotIndex = snapshotIndex ?? new Dictionary<ulong, SnapshotData>();
		_nodeIndex = nodeIndex ?? new Dictionary<ulong, DataSlice>();
		_nodeData = nodeData ?? new SpannableList<byte>();
	}

	public InMemoryRepository(Stream snapshotIndexSource, Stream nodeIndexSource, Stream nodeDataSource)
		: this(snapshotIndexSource, nodeIndexSource, nodeDataSource, null, null, null) { }

	internal InMemoryRepository(
		Stream snapshotIndexSource, Stream nodeIndexSource, Stream nodeDataSource,
		Dictionary<ulong, SnapshotData>? snapshotIndex = null,
		Dictionary<ulong, DataSlice>? nodeIndex = null,
		SpannableList<byte>? nodeData = null
	)
	{
		_snapshotIndex = snapshotIndex
			?? new Dictionary<ulong, SnapshotData>(PandoRepositoryIndexUtils.GetSnapshotIndexEntryCount(snapshotIndexSource));
		_nodeIndex = nodeIndex
			?? new Dictionary<ulong, DataSlice>(PandoRepositoryIndexUtils.GetNodeIndexEntryCount(nodeIndexSource));

		snapshotIndexSource.Seek(0, SeekOrigin.Begin);
		nodeIndexSource.Seek(0, SeekOrigin.Begin);
		nodeDataSource.Seek(0, SeekOrigin.Begin);

		while (PandoRepositoryIndexUtils.ReadNextSnapshotIndexEntry(snapshotIndexSource, out var hash, out var snapshotData))
		{
			AddSnapshotWithHashUnsafe(hash, snapshotData);
		}

		while (PandoRepositoryIndexUtils.ReadNextNodeIndexEntry(nodeIndexSource, out var hash, out var slice))
		{
			_nodeIndex.Add(hash, slice);
		}

		var buffer = new byte[nodeDataSource.Length];
		nodeDataSource.Read(buffer, 0, (int)nodeDataSource.Length);

		if (nodeData is not null)
		{
			_nodeData = nodeData;
			_nodeData.AddSpan(buffer);
		}
		else
		{
			_nodeData = new SpannableList<byte>(buffer);
		}
	}

	/// <remarks>This implementation is guaranteed not to insert duplicate nodes.</remarks>
	/// <inheritdoc/>
	public ulong AddNode(ReadOnlySpan<byte> bytes)
	{
		var hash = HashUtils.ComputeNodeHash(bytes);
		if (HasNode(hash)) return hash;

		AddNodeWithHashUnsafe(hash, bytes);
		return hash;
	}

	/// Adds a new node indexed with the given hash, containing the given bytes.
	/// <remarks>
	///     <para>This method is unsafe because the given hash and data might mismatch,
	///     and because it will blindly add the data to the node data collection
	///     even if it already exists in the collection.</para>
	///     <para>When calling this method, ensure the correct hash is given and
	///     call <see cref="HasNode"/> first to ensure that this is not a duplicate node.</para>
	/// </remarks>
	internal void AddNodeWithHashUnsafe(ulong hash, ReadOnlySpan<byte> bytes)
	{
		var dataSlice = _nodeData.AddSpan(bytes);
		_nodeIndex.Add(hash, dataSlice);
	}

	public ulong AddSnapshot(ulong parentHash, ulong rootNodeHash)
	{
		var hash = HashUtils.ComputeSnapshotHash(parentHash, rootNodeHash);
		if (HasSnapshot(hash)) return hash;

		AddSnapshotWithHashUnsafe(hash, parentHash, rootNodeHash);
		return hash;
	}

	/// <remarks>This method is unsafe because the given hash and data might mismatch.
	/// When calling this method, ensure the correct hash is given.</remarks>
	internal void AddSnapshotWithHashUnsafe(ulong hash, ulong parentHash, ulong rootNodeHash)
	{
		AddSnapshotWithHashUnsafe(hash, new SnapshotData(parentHash, rootNodeHash));
	}

	private void AddSnapshotWithHashUnsafe(ulong hash, SnapshotData snapshotData)
	{
		_snapshotIndex.Add(hash, snapshotData);
	}

	public bool HasNode(ulong hash) => _nodeIndex.ContainsKey(hash);

	public bool HasSnapshot(ulong hash) => _snapshotIndex.ContainsKey(hash);

	public T GetNode<T>(ulong hash, in IPandoNodeDeserializer<T> nodeDeserializer)
	{
		CheckNodeHash(hash);
		var (start, dataLength) = _nodeIndex[hash];
		var visitor = new RepositorySpanVisitor<T>(nodeDeserializer, this);
		return _nodeData.VisitSpan<RepositorySpanVisitor<T>, T>(start, dataLength, in visitor);
	}

	public int GetSizeOfNode(ulong hash)
	{
		CheckNodeHash(hash);
		var (_, dataLength) = _nodeIndex[hash];
		return dataLength;
	}

	public ulong GetSnapshotParent(ulong hash)
	{
		CheckSnapshotHash(hash);
		return _snapshotIndex[hash].ParentHash;
	}

	public ulong GetSnapshotRootNode(ulong hash)
	{
		CheckSnapshotHash(hash);
		return _snapshotIndex[hash].RootNodeHash;
	}

	public IEnumerable<SnapshotEntry> GetAllSnapshotEntries()
	{
		// ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
		foreach (var kvp in _snapshotIndex)
		{
			yield return new SnapshotEntry(kvp.Key, kvp.Value.ParentHash, kvp.Value.RootNodeHash);
		}
	}

	private void CheckNodeHash(ulong hash)
	{
		if (!HasNode(hash))
		{
			throw new HashNotFoundException($"The store does not contain a node with the requested hash {hash}");
		}
	}

	private void CheckSnapshotHash(ulong hash)
	{
		if (!HasSnapshot(hash))
		{
			throw new HashNotFoundException($"The store does not contain a snapshot with the requested hash {hash}");
		}
	}

	private readonly struct RepositorySpanVisitor<T> : ISpanVisitor<byte, T>
	{
		private readonly IPandoNodeDeserializer<T> _deserializer;
		private readonly IPandoRepository _repository;

		public RepositorySpanVisitor(IPandoNodeDeserializer<T> deserializer, IPandoRepository repository)
		{
			_deserializer = deserializer;
			_repository = repository;
		}

		public T Visit(ReadOnlySpan<byte> span) => _deserializer.Deserialize(span, _repository);
	}
}
