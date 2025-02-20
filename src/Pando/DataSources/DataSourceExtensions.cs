using System;
using System.Buffers.Binary;
using Pando.DataSources.Utils;

namespace Pando.DataSources;

public static class DataSourceExtensions
{
	/// Returns whether a node identified by the given hash buffer exists in the data source.
	public static bool HasNode(this INodeDataSource dataSource, ReadOnlySpan<byte> idBuffer)
	{
		var id = NodeId.FromBuffer(idBuffer);
		return dataSource.HasNode(id);
	}

	/// Gets the size in bytes of the node identified by the given hash buffer.
	public static int GetSizeOfNode(this INodeDataSource dataSource, ReadOnlySpan<byte> idBuffer)
	{
		var id = NodeId.FromBuffer(idBuffer);
		return dataSource.GetSizeOfNode(id);
	}

	/// Copies the binary representation of the node identified by the given hash buffer (little endian) into the given output Span.
	public static void CopyNodeBytesTo(this INodeDataSource dataSource, ReadOnlySpan<byte> idBuffer, Span<byte> outputBytes)
	{
		var hash = NodeId.FromBuffer(idBuffer);
		dataSource.CopyNodeBytesTo(hash, outputBytes);
	}

	/// Adds a node to the data sink and copies a hash that can be used to retrieve the node into the given hash buffer.
	public static void AddNode(this INodeDataSink dataSink, ReadOnlySpan<byte> bytes, Span<byte> idBuffer) =>
		dataSink.AddNode(bytes).CopyTo(idBuffer);
}
