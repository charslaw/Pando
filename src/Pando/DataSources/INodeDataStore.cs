using System;
using Pando.Exceptions;
using Pando.Repositories;

namespace Pando.DataSources;


public interface INodeDataStore : IReadOnlyNodeDataStore
{
	/// Adds a node to the data sink and returns a <see cref="NodeId"/> that can be used to retrieve the node.
	NodeId AddNode(ReadOnlySpan<byte> bytes);
}

public interface IReadOnlyNodeDataStore
{
	/// Returns whether a node identified by the given <see cref="NodeId"/> exists in the data source.
	bool HasNode(NodeId nodeId);

	/// Gets the size in bytes of the node identified by the given <see cref="NodeId"/>.
	/// <exception cref="NodeIdNotFoundException">If the given <paramref name="nodeId"/> is not found in the data source.</exception>
	int GetSizeOfNode(NodeId nodeId);

	/// Copies the binary representation of the node with the given <see cref="NodeId"/> into the given Span.
	/// <exception cref="NodeIdNotFoundException">If the given <paramref name="nodeId"/> is not found in the data source.</exception>
	/// <exception cref="ArgumentOutOfRangeException">If the given span is not large enough to contain the node data.</exception>
	void CopyNodeBytesTo(NodeId nodeId, Span<byte> outputBytes);
}
