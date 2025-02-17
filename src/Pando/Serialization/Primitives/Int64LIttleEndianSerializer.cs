using System;
using System.Buffers.Binary;
using Pando.DataSources;

namespace Pando.Serialization.Primitives;

/// Serializes/deserializes <c>long</c> values in little endian encoding (least significant byte first)
public class Int64LittleEndianSerializer : IPandoSerializer<long>
{
	/// <summary>A global default instance for <see cref="Int64LittleEndianSerializer"/></summary>
	public static Int64LittleEndianSerializer Default { get; } = new();

	public int SerializedSize => sizeof(long);
	public void Serialize(long value, Span<byte> buffer, INodeDataSink _) => BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
	public long Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource _) => BinaryPrimitives.ReadInt64LittleEndian(buffer);
}
