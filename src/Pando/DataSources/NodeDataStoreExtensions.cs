using System;
using Pando.Repositories;

namespace Pando.DataSources;

public static class NodeDataStoreExtensions
{
	/// Adds a node to the data store and populates the given <paramref name="idBuffer"/> with
	/// the bytes of the <see cref="NodeId"/> hash that can be used to retrieve the node.
	public static void AddNode(this IWriteOnlyNodeDataStore dataStore, ReadOnlySpan<byte> bytes, Span<byte> idBuffer)
	{
		ArgumentNullException.ThrowIfNull(dataStore);
		dataStore.AddNode(bytes).CopyTo(idBuffer);
	}

	/// Returns whether a node identified by the given <see cref="NodeId"/> hash exists in the data source.
	public static bool HasNode(this IReadOnlyNodeDataStore dataStore, ReadOnlySpan<byte> idBuffer)
	{
		ArgumentNullException.ThrowIfNull(dataStore);
		return dataStore.HasNode(NodeId.FromBuffer(idBuffer));
	}

	/// Gets the size in bytes of the node identified by the given <see cref="NodeId"/> hash.
	public static int GetSizeOfNode(this IReadOnlyNodeDataStore dataStore, ReadOnlySpan<byte> idBuffer)
	{
		ArgumentNullException.ThrowIfNull(dataStore);
		return dataStore.GetSizeOfNode(NodeId.FromBuffer(idBuffer));
	}

	/// Copies the binary representation of the node with the given <see cref="NodeId"/> hash into the given Span.
	public static void CopyNodeBytesTo(
		this IReadOnlyNodeDataStore dataStore,
		ReadOnlySpan<byte> idBuffer,
		Span<byte> outputBytes
	)
	{
		ArgumentNullException.ThrowIfNull(dataStore);
		dataStore.CopyNodeBytesTo(NodeId.FromBuffer(idBuffer), outputBytes);
	}
}
