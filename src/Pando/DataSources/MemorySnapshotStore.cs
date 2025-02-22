using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Pando.DataSources.Utils;
using Pando.Exceptions;
using Pando.Repositories;

namespace Pando.DataSources;

public class MemorySnapshotStore(Dictionary<SnapshotId, MemorySnapshotStore.TreeEntry> snapshotIndex) : ISnapshotDataStore
{
	public class TreeEntry(SnapshotId sourceParentId, SnapshotId targetParentId, NodeId rootNodeId, HashSet<SnapshotId>? children)
	{
		public readonly SnapshotId SourceParentId = sourceParentId;
		public readonly SnapshotId TargetParentId = targetParentId;
		public readonly NodeId RootNodeId = rootNodeId;
		public HashSet<SnapshotId>? Children = children;
	}

	public MemorySnapshotStore() : this(new Dictionary<SnapshotId, TreeEntry>()) { }

	public int SnapshotCount => snapshotIndex.Count;

	public SnapshotId? RootSnapshot { get; private set; }

	public bool HasSnapshot(SnapshotId snapshotId) => snapshotIndex.ContainsKey(snapshotId);

	public NodeId GetSnapshotRootNodeId(SnapshotId snapshotId)
	{
		if (snapshotIndex.TryGetValue(snapshotId, out var entry)) return entry.RootNodeId;

		throw new SnapshotIdNotFoundException(snapshotId, nameof(snapshotId));
	}

	public SnapshotData GetSnapshotData(SnapshotId snapshotId)
	{
		if (snapshotIndex.TryGetValue(snapshotId, out var entry))
		{
			return new SnapshotData(snapshotId, entry.SourceParentId, entry.TargetParentId, entry.RootNodeId);
		}

		throw new SnapshotIdNotFoundException(snapshotId, nameof(snapshotId));
	}

	public IEnumerable<SnapshotId> GetSnapshotChildren(SnapshotId snapshotId)
	{
		if (snapshotIndex.TryGetValue(snapshotId, out var entry)) return entry.Children ?? Enumerable.Empty<SnapshotId>();

		throw new SnapshotIdNotFoundException(snapshotId, nameof(snapshotId));
	}

	public SnapshotId GetSnapshotLeastCommonAncestor(SnapshotId id1, SnapshotId id2)
	{
		if (!snapshotIndex.ContainsKey(id1)) throw new SnapshotIdNotFoundException(id1, nameof(id1));
		if (!snapshotIndex.ContainsKey(id2)) throw new SnapshotIdNotFoundException(id2, nameof(id2));

		HashSet<SnapshotId> snapshot1Ancestors = [];
		var current = id1;
		while (current != SnapshotId.None)
		{
			snapshot1Ancestors.Add(current);
			current = snapshotIndex[current].SourceParentId;
		}

		current = id2;
		while (current != SnapshotId.None)
		{
			if (snapshot1Ancestors.Contains(current)) return current;
			current = snapshotIndex[current].SourceParentId;
		}

		// This should never happen since every node should descend from the root snapshot
		throw new InvalidOperationException("Snapshots have no common ancestor");
	}

	public void WalkTree(TreeEntryVisitor visitor)
	{
		if (!RootSnapshot.HasValue) throw new NoRootSnapshotException();

		var rootId = RootSnapshot.Value;
		var rootEntry = snapshotIndex[rootId];

		visitor(rootId, SnapshotId.None, SnapshotId.None, rootEntry.RootNodeId);

		var rootChildren = rootEntry.Children;
		if (rootChildren is null || rootChildren.Count == 0) return;

		var stack = ArrayPool<SnapshotId>.Shared.Rent(snapshotIndex.Count);
		var top = 0;

		try
		{
			foreach (var rootChild in rootChildren.Reverse()) stack[top++] = rootChild;

			while (top > 0)
			{
				var current = stack[--top];
				var currentEntry = snapshotIndex[current];
				visitor(current, currentEntry.SourceParentId, currentEntry.TargetParentId, currentEntry.RootNodeId);
				if (currentEntry.Children is null) continue;
				foreach (var child in currentEntry.Children.Reverse()) stack[top++] = child;
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
		if (!snapshotIndex.TryAdd(rootSnapshotId, new TreeEntry(SnapshotId.None, SnapshotId.None, rootNodeId, [])))
		{
			throw new AlreadyHasRootSnapshotException();
		}
		RootSnapshot = rootSnapshotId;
		return rootSnapshotId;
	}

	public SnapshotId AddSnapshot(NodeId rootNodeId, SnapshotId sourceSnapshotId, SnapshotId targetSnapshotId = default)
	{
		if (!snapshotIndex.TryGetValue(sourceSnapshotId, out var sourceParentEntry))
		{
			throw new SnapshotIdNotFoundException(sourceSnapshotId, nameof(sourceSnapshotId));
		}

		TreeEntry? targetParentEntry = null;
		if (targetSnapshotId != SnapshotId.None)
		{
			if (!snapshotIndex.TryGetValue(targetSnapshotId, out targetParentEntry))
			{
				throw new SnapshotIdNotFoundException(targetSnapshotId, nameof(targetSnapshotId));
			}
		}

		var snapshotId = HashUtils.ComputeSnapshotHash(rootNodeId, sourceSnapshotId, targetSnapshotId);

		if (snapshotIndex.ContainsKey(snapshotId)) return snapshotId;

		snapshotIndex.Add(snapshotId, new TreeEntry(sourceSnapshotId, targetSnapshotId, rootNodeId, null));
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
