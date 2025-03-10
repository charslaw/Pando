using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pando.DataSources.Utils;
using Pando.Exceptions;
using Pando.Repositories;

namespace Pando.DataSources;

public class MemoryNodeStore : INodeDataStore
{
	private readonly Dictionary<NodeId, Range> _nodeIndex;
	private readonly SpannableList<byte> _nodeData;

	public MemoryNodeStore()
	{
		_nodeIndex = new Dictionary<NodeId, Range>();
		_nodeData = new SpannableList<byte>();
	}

	internal MemoryNodeStore(Dictionary<NodeId, Range>? nodeIndex = null, SpannableList<byte>? nodeData = null)
	{
		_nodeIndex = nodeIndex ?? new Dictionary<NodeId, Range>();
		_nodeData = nodeData ?? new SpannableList<byte>();
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
