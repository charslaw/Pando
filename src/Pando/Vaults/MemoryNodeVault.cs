using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Pando.Exceptions;
using Pando.Persistors;
using Pando.Repositories;
using Pando.Vaults.Utils;

namespace Pando.Vaults;

public class MemoryNodeVault : INodeVault
{
	private readonly Dictionary<NodeId, Range> _nodeIndex;
	private readonly SpannableList<byte> _nodeData;
	private readonly INodePersistor? _persistor;

	public MemoryNodeVault()
	{
		_nodeIndex = new Dictionary<NodeId, Range>();
		_nodeData = new SpannableList<byte>();
		_persistor = null;
	}

	private MemoryNodeVault(INodePersistor persistor, Dictionary<NodeId, Range> nodeIndex, SpannableList<byte> nodeData)
	{
		_persistor = persistor;
		_nodeIndex = nodeIndex;
		_nodeData = nodeData;
	}

	public static async Task<MemoryNodeVault> Create(INodePersistor persistor)
	{
		ArgumentNullException.ThrowIfNull(persistor);

		var (index, data) = await persistor.LoadNodeData().ConfigureAwait(false);

		var nodeIndex = index as Dictionary<NodeId, Range> ?? index.ToDictionary();
		var nodeData = new SpannableList<byte>(data as byte[] ?? data.ToArray());

		return new MemoryNodeVault(persistor, nodeIndex, nodeData);
	}

	internal MemoryNodeVault(Dictionary<NodeId, Range>? nodeIndex = null, SpannableList<byte>? nodeData = null)
	{
		_nodeIndex = nodeIndex ?? new Dictionary<NodeId, Range>();
		_nodeData = nodeData ?? new SpannableList<byte>();
		_persistor = null;
	}

	/// <remarks>This implementation is guaranteed not to insert duplicate nodes.</remarks>
	/// <inheritdoc/>
	public NodeId AddNode(ReadOnlySpan<byte> bytes)
	{
		_ = TryAddNode(bytes, out var nodeId);
		return nodeId;
	}

	public bool TryAddNode(ReadOnlySpan<byte> bytes, out NodeId nodeId)
	{
		nodeId = HashUtils.ComputeNodeHash(bytes);
		if (HasNode(nodeId))
			return false;

		var range = _nodeData.AddSpan(bytes);
		_nodeIndex.Add(nodeId, range);
		_persistor?.PersistNode(nodeId, bytes);
		return true;
	}

	public bool HasNode(NodeId nodeId) => _nodeIndex.ContainsKey(nodeId);

	public int GetSizeOfNode(NodeId nodeId)
	{
		var (_, dataLength) = GetNodeRange(nodeId).GetOffsetAndLength(_nodeData.Count);
		return dataLength;
	}

	public void CopyNodeBytesTo(NodeId nodeId, Span<byte> outputBytes)
	{
		_nodeData.CopyTo(GetNodeRange(nodeId), outputBytes);
	}

	private Range GetNodeRange(NodeId nodeId, [CallerArgumentExpression(nameof(nodeId))] string? paramName = null)
	{
		if (!_nodeIndex.TryGetValue(nodeId, out var range))
			throw new NodeIdNotFoundException(nodeId, paramName!);

		return range;
	}
}
