using System;
using System.Buffers.Binary;

namespace Pando.DataSources;

public static class DataSourceExtensions
{
	/// Returns whether a node identified by the given hash buffer exists in the data source.
	public static bool HasNode(this INodeDataSource dataSource, ReadOnlySpan<byte> hashBuffer)
	{
		var hash = BinaryPrimitives.ReadUInt64LittleEndian(hashBuffer);
		return dataSource.HasNode(hash);
	}

	/// Gets the size in bytes of the node identified by the given hash buffer.
	public static int GetSizeOfNode(this INodeDataSource dataSource, ReadOnlySpan<byte> hashBuffer)
	{
		var hash = BinaryPrimitives.ReadUInt64LittleEndian(hashBuffer);
		return dataSource.GetSizeOfNode(hash);
	}

	/// Copies the binary representation of the node identified by the given hash buffer (little endian) into the given output Span.
	public static void CopyNodeBytesTo(this INodeDataSource dataSource, ReadOnlySpan<byte> hashBuffer, Span<byte> outputBytes)
	{
		var hash = BinaryPrimitives.ReadUInt64LittleEndian(hashBuffer);
		dataSource.CopyNodeBytesTo(hash, outputBytes);
	}

	/// Adds a node to the data sink and copies a hash that can be used to retrieve the node into the given hash buffer.
	public static void AddNode(this INodeDataSink dataSink, ReadOnlySpan<byte> bytes, Span<byte> hashBuffer)
	{
		var hash = dataSink.AddNode(bytes);
		BinaryPrimitives.WriteUInt64LittleEndian(hashBuffer, hash);
	}
}
