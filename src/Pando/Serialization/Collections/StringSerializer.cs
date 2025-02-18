using System;
using System.Buffers.Binary;
using System.Text;
using Pando.DataSources;

namespace Pando.Serialization.Collections;

/// Serializes a string using a given encoding
public class StringSerializer(Encoding encoding) : IPandoSerializer<string>
{
	/// A default serializer for strings that uses the UTF8 encoding.
	public static StringSerializer UTF8 { get; } = new(Encoding.UTF8);

	/// A default serializer for strings that uses the ASCII encoding.
	public static StringSerializer ASCII { get; } = new(Encoding.ASCII);

	public int SerializedSize => sizeof(ulong);

	public void Serialize(string value, Span<byte> buffer, INodeDataSink dataSink)
	{
		var bytesSize = encoding.GetByteCount(value);
		Span<byte> elementBytes = stackalloc byte[bytesSize];
		encoding.GetBytes(value, elementBytes);

		var nodeHash = dataSink.AddNode(elementBytes);
		BinaryPrimitives.WriteUInt64LittleEndian(buffer, nodeHash);
	}

	public string Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var nodeHash = BinaryPrimitives.ReadUInt64LittleEndian(buffer);
		var nodeDataSize = dataSource.GetSizeOfNode(nodeHash);
		Span<byte> elementBytes = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(nodeHash, elementBytes);
		return encoding.GetString(elementBytes);
	}
}
