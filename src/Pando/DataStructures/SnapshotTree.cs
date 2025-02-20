using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Pando.DataSources.Utils;
using Pando.Exceptions;

namespace Pando.DataStructures;

public class SnapshotTree : ISnapshotTree
{
	private class TreeEntry(SnapshotParents parents, HashSet<SnapshotId>? children)
	{
		public readonly SnapshotParents Parents = parents;
		public HashSet<SnapshotId>? Children = children;
	}

	private readonly Dictionary<SnapshotId, TreeEntry> _entries = new();

	public SnapshotId? RootSnapshot { get; private set; }

	public bool HasSnapshot(SnapshotId snapshotId) => _entries.ContainsKey(snapshotId);

	public IEnumerable<SnapshotId> GetSnapshotChildren(SnapshotId snapshotId)
	{
		if (_entries.TryGetValue(snapshotId, out var entry)) return entry.Children ?? Enumerable.Empty<SnapshotId>();

		throw new SnapshotIdNotFoundException(snapshotId, nameof(snapshotId));
	}

	public SnapshotParents GetSnapshotParents(SnapshotId snapshotId)
	{
		if (_entries.TryGetValue(snapshotId, out var entry)) return entry.Parents;

		throw new SnapshotIdNotFoundException(snapshotId, nameof(snapshotId));
	}

	public IEnumerable<(SnapshotId snapshotId, SnapshotParents parents)> EnumerateDepthFirst()
	{
		if (!RootSnapshot.HasValue) throw new NoRootSnapshotException();

		return iter(RootSnapshot.Value, _entries);

		static IEnumerable<(SnapshotId, SnapshotParents)> iter(SnapshotId rootId, Dictionary<SnapshotId, TreeEntry> entries)
		{
			var rootChildren = entries[rootId].Children;
			if (rootChildren is null || rootChildren.Count == 0) yield break;

			var stack = ArrayPool<SnapshotId>.Shared.Rent(entries.Count);
			var top = 0;

			try
			{
				yield return (rootId, new SnapshotParents(SnapshotId.None, SnapshotId.None));
				foreach (var rootChild in rootChildren.Reverse()) stack[top++] = rootChild;

				while (top > 0)
				{
					var current = stack[--top];
					var currentEntry = entries[current];
					yield return (current, currentEntry.Parents);
					if (currentEntry.Children is null) continue;
					foreach (var child in currentEntry.Children.Reverse()) stack[top++] = child;
				}
			}
			finally
			{
				ArrayPool<SnapshotId>.Shared.Return(stack);
			}
		}
	}

	public void AddRootSnapshot(SnapshotId rootSnapshotId)
	{
		if (!_entries.TryAdd(rootSnapshotId, new TreeEntry(new SnapshotParents(SnapshotId.None, SnapshotId.None), null)))
		{
			throw new AlreadyHasRootSnapshotException();
		}
		RootSnapshot = rootSnapshotId;
	}

	public void AddSnapshot(SnapshotId snapshotId, SnapshotParents parents)
	{
		if (_entries.ContainsKey(snapshotId)) return;

		ref var sourceParentEntry = ref CollectionsMarshal.GetValueRefOrNullRef(_entries, parents.SourceParentSnapshotId);
		if (Unsafe.IsNullRef(ref sourceParentEntry))
		{
			throw new SnapshotIdNotFoundException(parents.SourceParentSnapshotId, nameof(parents.SourceParentSnapshotId));
		}

		ref TreeEntry targetParentEntry = ref Unsafe.NullRef<TreeEntry>();
		if (parents.TargetParentSnapshotId != SnapshotId.None)
		{
			targetParentEntry = ref CollectionsMarshal.GetValueRefOrNullRef(_entries, parents.SourceParentSnapshotId);
			if (Unsafe.IsNullRef(ref targetParentEntry))
			{
				throw new SnapshotIdNotFoundException(parents.TargetParentSnapshotId, nameof(parents.TargetParentSnapshotId));
			}
		}

		_entries.Add(snapshotId, new TreeEntry(parents, null));
		sourceParentEntry.Children ??= [];
		sourceParentEntry.Children.Add(snapshotId);

		if (Unsafe.IsNullRef(ref targetParentEntry)) return;

		targetParentEntry.Children ??= [];
		targetParentEntry.Children.Add(snapshotId);
	}

	public void AddSnapshot(SnapshotId snapshotId, SnapshotId parentSnapshotId)
	{
		if (_entries.ContainsKey(snapshotId)) return;

		if (!_entries.TryGetValue(parentSnapshotId, out var parentEntry))
		{
			throw new SnapshotIdNotFoundException(parentSnapshotId, nameof(parentSnapshotId));
		}

		_entries.Add(snapshotId, new TreeEntry(new SnapshotParents(parentSnapshotId, SnapshotId.None), null));
		parentEntry.Children ??= [];
		parentEntry.Children.Add(snapshotId);
	}
}
