using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Pando.DataSources.Utils;
using Pando.Exceptions;
using Pando.Repositories;

namespace Pando.DataSources;

public class MemorySnapshotStore : ISnapshotDataStore
{
	private readonly Dictionary<SnapshotId, TreeEntry> _snapshotIndex;

	internal class TreeEntry(
		SnapshotId sourceParentId,
		SnapshotId targetParentId,
		NodeId rootNodeId,
		HashSet<SnapshotId>? children
	)
	{
		public SnapshotId SourceParentId { get; } = sourceParentId;
		public SnapshotId TargetParentId { get; } = targetParentId;
		public NodeId RootNodeId { get; } = rootNodeId;
		public HashSet<SnapshotId>? Children { get; set; } = children;
	}

	public MemorySnapshotStore()
		: this(new Dictionary<SnapshotId, TreeEntry>()) { }

	internal MemorySnapshotStore(Dictionary<SnapshotId, TreeEntry> snapshotIndex)
	{
		_snapshotIndex = snapshotIndex;
	}

	public int SnapshotCount => _snapshotIndex.Count;

	public SnapshotId? RootSnapshot { get; private set; }

	public bool HasSnapshot(SnapshotId snapshotId) => _snapshotIndex.ContainsKey(snapshotId);

	public NodeId GetSnapshotRootNodeId(SnapshotId snapshotId)
	{
		if (_snapshotIndex.TryGetValue(snapshotId, out var entry))
			return entry.RootNodeId;

		throw new SnapshotIdNotFoundException(snapshotId, nameof(snapshotId));
	}

	public SnapshotData GetSnapshotData(SnapshotId snapshotId)
	{
		if (_snapshotIndex.TryGetValue(snapshotId, out var entry))
		{
			return new SnapshotData(snapshotId, entry.SourceParentId, entry.TargetParentId, entry.RootNodeId);
		}

		throw new SnapshotIdNotFoundException(snapshotId, nameof(snapshotId));
	}

	public IEnumerable<SnapshotId> GetSnapshotChildren(SnapshotId snapshotId)
	{
		if (_snapshotIndex.TryGetValue(snapshotId, out var entry))
			return entry.Children ?? Enumerable.Empty<SnapshotId>();

		throw new SnapshotIdNotFoundException(snapshotId, nameof(snapshotId));
	}

	public SnapshotId GetSnapshotLeastCommonAncestor(SnapshotId id1, SnapshotId id2)
	{
		if (!_snapshotIndex.ContainsKey(id1))
			throw new SnapshotIdNotFoundException(id1, nameof(id1));
		if (!_snapshotIndex.ContainsKey(id2))
			throw new SnapshotIdNotFoundException(id2, nameof(id2));

		HashSet<SnapshotId> snapshot1Ancestors = [];
		var current = id1;
		while (current != SnapshotId.None)
		{
			snapshot1Ancestors.Add(current);
			current = _snapshotIndex[current].SourceParentId;
		}

		current = id2;
		while (current != SnapshotId.None)
		{
			if (snapshot1Ancestors.Contains(current))
				return current;
			current = _snapshotIndex[current].SourceParentId;
		}

		// This should never happen since every node should descend from the root snapshot
		throw new InvalidOperationException("Snapshots have no common ancestor");
	}

	public void WalkTree(TreeEntryVisitor visitor)
	{
		ArgumentNullException.ThrowIfNull(visitor);
		if (!RootSnapshot.HasValue)
			throw new NoRootSnapshotException();

		var rootId = RootSnapshot.Value;
		var rootEntry = _snapshotIndex[rootId];

		visitor(rootId, SnapshotId.None, SnapshotId.None, rootEntry.RootNodeId);

		var rootChildren = rootEntry.Children;
		if (rootChildren is null || rootChildren.Count == 0)
			return;

		var stack = ArrayPool<SnapshotId>.Shared.Rent(_snapshotIndex.Count);
		var top = 0;

		try
		{
			foreach (var rootChild in rootChildren.Reverse())
				stack[top++] = rootChild;

			while (top > 0)
			{
				var current = stack[--top];
				var currentEntry = _snapshotIndex[current];
				visitor(current, currentEntry.SourceParentId, currentEntry.TargetParentId, currentEntry.RootNodeId);
				if (currentEntry.Children is null)
					continue;
				foreach (var child in currentEntry.Children.Reverse())
					stack[top++] = child;
			}
		}
		finally
		{
			ArrayPool<SnapshotId>.Shared.Return(stack);
		}
	}

	public SnapshotId AddRootSnapshot(NodeId rootNodeId)
	{
		var rootSnapshotId = HashUtils.ComputeSnapshotHash(rootNodeId, SnapshotId.None, SnapshotId.None);
		if (!_snapshotIndex.TryAdd(rootSnapshotId, new TreeEntry(SnapshotId.None, SnapshotId.None, rootNodeId, [])))
		{
			throw new AlreadyHasRootSnapshotException();
		}
		RootSnapshot = rootSnapshotId;
		return rootSnapshotId;
	}

	public SnapshotId AddSnapshot(NodeId rootNodeId, SnapshotId sourceSnapshotId, SnapshotId targetSnapshotId = default)
	{
		if (!_snapshotIndex.TryGetValue(sourceSnapshotId, out var sourceParentEntry))
		{
			throw new SnapshotIdNotFoundException(sourceSnapshotId, nameof(sourceSnapshotId));
		}

		TreeEntry? targetParentEntry = null;
		if (targetSnapshotId != SnapshotId.None)
		{
			if (!_snapshotIndex.TryGetValue(targetSnapshotId, out targetParentEntry))
			{
				throw new SnapshotIdNotFoundException(targetSnapshotId, nameof(targetSnapshotId));
			}
		}

		var snapshotId = HashUtils.ComputeSnapshotHash(rootNodeId, sourceSnapshotId, targetSnapshotId);

		if (_snapshotIndex.ContainsKey(snapshotId))
			return snapshotId;

		_snapshotIndex.Add(snapshotId, new TreeEntry(sourceSnapshotId, targetSnapshotId, rootNodeId, null));
		sourceParentEntry.Children ??= [];
		sourceParentEntry.Children.Add(snapshotId);

		if (targetParentEntry is not null)
		{
			targetParentEntry.Children ??= [];
			targetParentEntry.Children.Add(snapshotId);
		}

		return snapshotId;
	}
}
