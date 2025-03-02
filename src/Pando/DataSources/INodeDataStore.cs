using System;
using Pando.Exceptions;
using Pando.Repositories;

namespace Pando.DataSources;


public interface INodeDataStore : IReadOnlyNodeDataStore
{
	/// Adds a node to the data store and populates the given <paramref name="idBuffer"/> with
	/// the bytes of the <see cref="NodeId"/> hash that can be used to retrieve the node.
	void AddNode(ReadOnlySpan<byte> bytes, Span<byte> idBuffer);

	/// Adds a node to the data store and returns a <see cref="NodeId"/> that can be used to retrieve the node.
	NodeId AddNode(ReadOnlySpan<byte> bytes);
}

public interface IReadOnlyNodeDataStore
{
	/// Returns whether a node identified by the given <see cref="NodeId"/> hash exists in the data source.
	bool HasNode(ReadOnlySpan<byte> idBuffer);

	/// Returns whether a node identified by the given <see cref="NodeId"/> exists in the data source.
	bool HasNode(NodeId nodeId);

	/// Gets the size in bytes of the node identified by the given <see cref="NodeId"/> hash.
	/// <exception cref="NodeIdNotFoundException">If the data source does not contain a node identified by the given hash.</exception>
	int GetSizeOfNode(ReadOnlySpan<byte> idBuffer);

	/// Gets the size in bytes of the node identified by the given <see cref="NodeId"/>.
	/// <exception cref="NodeIdNotFoundException">
	/// If the data source does not contain a node identified by the given <paramref name="nodeId"/>.
	/// </exception>
	int GetSizeOfNode(NodeId nodeId);

	/// Copies the binary representation of the node with the given <see cref="NodeId"/> hash into the given Span.
	/// <exception cref="NodeIdNotFoundException">If the data source does not contain a node identified by the given hash.</exception>
	/// <exception cref="ArgumentOutOfRangeException">If the given span is not large enough to contain the node data.</exception>
	void CopyNodeBytesTo(ReadOnlySpan<byte> idBuffer, Span<byte> outputBytes);

	/// Copies the binary representation of the node with the given <see cref="NodeId"/> into the given Span.
	/// <exception cref="NodeIdNotFoundException">If the given <paramref name="nodeId"/> is not found in the data source.</exception>
	/// <exception cref="ArgumentOutOfRangeException">If the given span is not large enough to contain the node data.</exception>
	void CopyNodeBytesTo(NodeId nodeId, Span<byte> outputBytes);
}
