using System;
using System.Collections.Generic;
using Pando.Repositories;

namespace Pando.Persistors;

public interface INodePersistor
{
	/// Writes the given node to the appropriate persistence method
	void PersistNode(NodeId nodeId, ReadOnlySpan<byte> data);

	/// Loads the node index and data from the persistence method and returns it
	(IEnumerable<KeyValuePair<NodeId, Range>>, IEnumerable<byte>) LoadNodeData();
}
