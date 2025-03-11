using System;
using System.Collections.Generic;
using System.Linq;
using Pando.Repositories;

namespace Pando.Vaults;

public class CompositeNodePersistor : INodePersistor
{
	private readonly INodePersistor[] _persistors;
	private readonly INodePersistor _primaryPersistor;

	public CompositeNodePersistor(IEnumerable<INodePersistor> persistors)
	{
		_persistors = persistors.ToArray();
		_primaryPersistor = _persistors[0];
	}

	public void PersistNode(NodeId nodeId, ReadOnlySpan<byte> data)
	{
		foreach (var persistor in _persistors)
		{
			persistor.PersistNode(nodeId, data);
		}
	}

	public (Dictionary<NodeId, Range>, byte[]) LoadNodeData()
	{
		return _primaryPersistor.LoadNodeData();
	}
}
