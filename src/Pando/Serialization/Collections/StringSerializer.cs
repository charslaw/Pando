using System;
using System.Buffers.Binary;
using System.Text;
using Pando.DataSources;
using Pando.DataSources.Utils;

namespace Pando.Serialization.Collections;

/// Serializes a string using a given encoding
public class StringSerializer(Encoding encoding) : IPandoSerializer<string>
{
	/// A default serializer for strings that uses the UTF8 encoding.
	public static StringSerializer UTF8 { get; } = new(Encoding.UTF8);

	/// A default serializer for strings that uses the ASCII encoding.
	public static StringSerializer ASCII { get; } = new(Encoding.ASCII);

	public int SerializedSize => NodeId.SIZE;

	public void Serialize(string value, Span<byte> buffer, INodeDataSink dataSink)
	{
		var bytesSize = encoding.GetByteCount(value);
		Span<byte> elementBytes = stackalloc byte[bytesSize];
		encoding.GetBytes(value, elementBytes);

		dataSink.AddNode(elementBytes, buffer);
	}

	public string Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var nodeDataSize = dataSource.GetSizeOfNode(buffer);
		Span<byte> elementBytes = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(buffer, elementBytes);
		return encoding.GetString(elementBytes);
	}
}
