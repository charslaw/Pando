using System;
using System.Collections.Generic;
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

	public NodeId AddNode(ReadOnlySpan<byte> bytes)
	{
		var nodeId = HashUtils.ComputeNodeHash(bytes);

		if (_mainDataSource.HasNode(nodeId)) return nodeId;

		_mainDataSource.AddNodeWithIdUnsafe(nodeId, bytes);
		_persistentDataSource.AddNodeWithHashUnsafe(nodeId, bytes);
		return nodeId;
	}

	public SnapshotId AddSnapshot(SnapshotId parentSnapshotId, NodeId rootNodeId)
	{
		var snapshotId = HashUtils.ComputeSnapshotHash(parentSnapshotId, rootNodeId);

		if (_mainDataSource.HasSnapshot(snapshotId)) return snapshotId;

		_mainDataSource.AddSnapshotWithIdUnsafe(snapshotId, parentSnapshotId, rootNodeId);
		_persistentDataSource.AddSnapshotWithHashUnsafe(snapshotId, parentSnapshotId, rootNodeId);
		return snapshotId;
	}

	public bool HasNode(NodeId nodeId) => _mainDataSource.HasNode(nodeId);
	public int GetSizeOfNode(NodeId nodeId) => _mainDataSource.GetSizeOfNode(nodeId);
	public void CopyNodeBytesTo(NodeId nodeId, Span<byte> outputBytes) => _mainDataSource.CopyNodeBytesTo(nodeId, outputBytes);

	public bool HasSnapshot(SnapshotId snapshotId) => _mainDataSource.HasSnapshot(snapshotId);
	public int SnapshotCount => _mainDataSource.SnapshotCount;
	public SnapshotId GetSnapshotParent(SnapshotId snapshotId) => _mainDataSource.GetSnapshotParent(snapshotId);
	public SnapshotId GetSnapshotLeastCommonAncestor(SnapshotId id1, SnapshotId id2) => _mainDataSource.GetSnapshotLeastCommonAncestor(id1, id2);

	public NodeId GetSnapshotRootNode(SnapshotId snapshotId) => _mainDataSource.GetSnapshotRootNode(snapshotId);
	public IReadOnlySet<SnapshotId> GetLeafSnapshotIds() => _mainDataSource.GetLeafSnapshotIds();

	public void Dispose() => _persistentDataSource.Dispose();
}
