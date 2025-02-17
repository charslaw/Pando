using System;
using System.Buffers.Binary;
using Pando.DataSources;

namespace Pando.Serialization.Primitives;

/// Serializes/deserializes <c>int</c> values in little endian encoding (least significant byte first)
public class Int32LittleEndianSerializer : IPandoSerializer<int>
{
	/// <summary>A global default instance for <see cref="Int32LittleEndianSerializer"/></summary>
	public static Int32LittleEndianSerializer Default { get; } = new();

	public int SerializedSize => sizeof(int);

	public void Serialize(int value, Span<byte> buffer, INodeDataSink _) => BinaryPrimitives.WriteInt32LittleEndian(buffer, value);

	public int Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource _) => BinaryPrimitives.ReadInt32LittleEndian(buffer);
}
