using System.Collections.Generic;
using Pando.Repositories;

namespace Pando.Persistors;

public interface ISnapshotPersistor
{
	void PersistSnapshot(
		SnapshotId snapshotId,
		SnapshotId sourceParentId,
		SnapshotId targetParentId,
		NodeId rootNodeId
	);

	IEnumerable<
		KeyValuePair<SnapshotId, (SnapshotId sourceParentId, SnapshotId targetParentId, NodeId rootNodeId)>
	> LoadSnapshotIndex();
}
