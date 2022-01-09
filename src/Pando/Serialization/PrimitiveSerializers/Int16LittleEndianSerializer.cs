using System;
using System.Buffers.Binary;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes/deserializes <c>short</c> values in little endian encoding (least significant byte first)
public class Int16LittleEndianSerializer : FixedSizeBaseSerializer<short>
{
	/// <summary>A global default instance for <see cref="Int16LittleEndianSerializer"/></summary>
	public static Int16LittleEndianSerializer Default { get; } = new();

	protected override int FixedSize => sizeof(short);
	protected override void SerializeInner(short value, Span<byte> slice) => BinaryPrimitives.WriteInt16LittleEndian(slice, value);
	protected override short DeserializeInner(ReadOnlySpan<byte> slice) => BinaryPrimitives.ReadInt16LittleEndian(slice);
}
