using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Pando.DataSources.Utils;
using Pando.Exceptions;

namespace Pando.DataSources;

public class MemoryDataSource : IDataSource
{
	private readonly Dictionary<ulong, SnapshotData> _snapshotIndex;
	private readonly HashSet<ulong> _leafSnapshots;
	private readonly Dictionary<ulong, DataSlice> _nodeIndex;
	private readonly SpannableList<byte> _nodeData;

	public MemoryDataSource()
	{
		_snapshotIndex = new Dictionary<ulong, SnapshotData>();
		_leafSnapshots = new HashSet<ulong>();
		_nodeIndex = new Dictionary<ulong, DataSlice>();
		_nodeData = new SpannableList<byte>();
	}

	public MemoryDataSource(Stream snapshotIndexSource, Stream leafSnapshotsSource, Stream nodeIndexSource, Stream nodeDataSource)
		: this(
			snapshotIndexSource: snapshotIndexSource,
			leafSnapshotsSource: leafSnapshotsSource,
			nodeIndexSource: nodeIndexSource,
			nodeDataSource: nodeDataSource,
			null, null, null
		) { }

	internal MemoryDataSource(
		Dictionary<ulong, SnapshotData>? snapshotIndex = null,
		Dictionary<ulong, DataSlice>? nodeIndex = null,
		SpannableList<byte>? nodeData = null
	)
	{
		_snapshotIndex = snapshotIndex ?? new Dictionary<ulong, SnapshotData>();
		_leafSnapshots = new HashSet<ulong>();
		_nodeIndex = nodeIndex ?? new Dictionary<ulong, DataSlice>();
		_nodeData = nodeData ?? new SpannableList<byte>();
	}


	internal MemoryDataSource(
		Stream snapshotIndexSource, Stream leafSnapshotsSource, Stream nodeIndexSource, Stream nodeDataSource,
		Dictionary<ulong, SnapshotData>? snapshotIndex = null,
		Dictionary<ulong, DataSlice>? nodeIndex = null,
		SpannableList<byte>? nodeData = null
	)
	{
		_snapshotIndex = StreamUtils.SnapshotIndex.PopulateSnapshotIndex(snapshotIndexSource, snapshotIndex);
		_leafSnapshots = StreamUtils.LeafSnapshotSet.PopulateLeafSnapshotsSet(leafSnapshotsSource);
		_nodeIndex = StreamUtils.NodeIndex.PopulateNodeIndex(nodeIndexSource, nodeIndex);
		_nodeData = StreamUtils.NodeData.PopulateNodeData(nodeDataSource, nodeData);
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

		// Parent is by definition no longer a leaf node
		_leafSnapshots.Remove(snapshotData.ParentHash);
		// Newly add snapshot is by definition a leaf node
		_leafSnapshots.Add(hash);
	}

	public bool HasNode(ulong hash) => _nodeIndex.ContainsKey(hash);

	public bool HasSnapshot(ulong hash) => _snapshotIndex.ContainsKey(hash);

	public int SnapshotCount => _snapshotIndex.Count;

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

	public IImmutableSet<ulong> GetLeafSnapshotHashes() => _leafSnapshots.ToImmutableHashSet();

	private void CheckNodeHash(ulong hash)
	{
		if (!HasNode(hash))
		{
			throw new HashNotFoundException($"The data source does not contain a node with the requested hash {hash}");
		}
	}

	private void CheckSnapshotHash(ulong hash)
	{
		if (!HasSnapshot(hash))
		{
			throw new HashNotFoundException($"The data source does not contain a snapshot with the requested hash {hash}");
		}
	}

	private readonly struct RepositorySpanVisitor<T> : ISpanVisitor<byte, T>
	{
		private readonly IPandoNodeDeserializer<T> _deserializer;
		private readonly IDataSource _repository;

		public RepositorySpanVisitor(IPandoNodeDeserializer<T> deserializer, IDataSource repository)
		{
			_deserializer = deserializer;
			_repository = repository;
		}

		public T Visit(ReadOnlySpan<byte> span) => _deserializer.Deserialize(span, _repository);
	}
}
