using System;
using Pando.Exceptions;
using Pando.Repositories;

namespace Pando.Vaults;

public interface INodeVault : IWriteOnlyNodeVault, IReadOnlyNodeVault;

public interface IWriteOnlyNodeVault
{
	/// Adds a node to the data store and returns a <see cref="NodeId"/> that can be used to retrieve the node.
	NodeId AddNode(ReadOnlySpan<byte> bytes);

	/// Attempts to add the given node to the data store, returning true if it was added or false if the node already
	/// existed in the data store.
	bool TryAddNode(ReadOnlySpan<byte> bytes, out NodeId nodeId);
}

public interface IReadOnlyNodeVault
{
	/// Returns whether a node identified by the given <see cref="NodeId"/> exists in the data source.
	bool HasNode(NodeId nodeId);

	/// Gets the size in bytes of the node identified by the given <see cref="NodeId"/>.
	/// <exception cref="NodeIdNotFoundException">
	/// If the data source does not contain a node identified by the given <paramref name="nodeId"/>.
	/// </exception>
	int GetSizeOfNode(NodeId nodeId);

	/// Copies the binary representation of the node with the given <see cref="NodeId"/> into the given Span.
	/// <exception cref="NodeIdNotFoundException">If the given <paramref name="nodeId"/> is not found in the data source.</exception>
	/// <exception cref="ArgumentOutOfRangeException">If the given span is not large enough to contain the node data.</exception>
	void CopyNodeBytesTo(NodeId nodeId, Span<byte> outputBytes);
}
