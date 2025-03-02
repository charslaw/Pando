using System;
using Pando.Repositories;

namespace Pando.DataSources;

public static class DataStoreExtensions
{
	/// Returns whether a node identified by the given hash buffer exists in the data source.
	public static bool HasNode(this IReadOnlyNodeDataStore dataStore, ReadOnlySpan<byte> idBuffer)
	{
		var id = NodeId.FromBuffer(idBuffer);
		return dataStore.HasNode(id);
	}

	/// Gets the size in bytes of the node identified by the given hash buffer.
	public static int GetSizeOfNode(this IReadOnlyNodeDataStore dataStore, ReadOnlySpan<byte> idBuffer)
	{
		var id = NodeId.FromBuffer(idBuffer);
		return dataStore.GetSizeOfNode(id);
	}

	/// Copies the binary representation of the node identified by the given hash buffer (little endian) into the given output Span.
	public static void CopyNodeBytesTo(this IReadOnlyNodeDataStore dataStore, ReadOnlySpan<byte> idBuffer, Span<byte> outputBytes)
	{
		var hash = NodeId.FromBuffer(idBuffer);
		dataStore.CopyNodeBytesTo(hash, outputBytes);
	}

	/// Adds a node to the data sink and copies a hash that can be used to retrieve the node into the given hash buffer.
	public static void AddNode(this INodeDataStore dataStore, ReadOnlySpan<byte> bytes, Span<byte> idBuffer) =>
		dataStore.AddNode(bytes).CopyTo(idBuffer);
}
