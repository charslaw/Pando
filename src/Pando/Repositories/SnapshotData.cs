namespace Pando.Repositories;

public record struct SnapshotData(
	SnapshotId Id,
	SnapshotId SourceParentId,
	SnapshotId TargetParentId,
	NodeId RootNodeId
);
