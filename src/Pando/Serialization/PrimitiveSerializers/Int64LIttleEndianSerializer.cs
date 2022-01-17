using System;
using System.Buffers.Binary;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes/deserializes <c>long</c> values in little endian encoding (least significant byte first)
public class Int64LittleEndianSerializer : FixedSizeBaseSerializer<long>
{
	/// <summary>A global default instance for <see cref="Int64LittleEndianSerializer"/></summary>
	public static Int64LittleEndianSerializer Default { get; } = new();

	public override int FixedSize => sizeof(long);
	protected override void SerializeInner(long value, Span<byte> slice) => BinaryPrimitives.WriteInt64LittleEndian(slice, value);
	protected override long DeserializeInner(ReadOnlySpan<byte> slice) => BinaryPrimitives.ReadInt64LittleEndian(slice);
}
