using System;
using System.Buffers.Binary;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes/deserializes <c>int</c> values in little endian encoding (least significant byte first)
public class Int32LittleEndianSerializer : FixedSizeBaseSerializer<int>
{
	/// <summary>A global default instance for <see cref="Int32LittleEndianSerializer"/></summary>
	public static Int32LittleEndianSerializer Default { get; } = new();

	public override int FixedSize => sizeof(int);
	protected override void SerializeInner(int value, Span<byte> slice) => BinaryPrimitives.WriteInt32LittleEndian(slice, value);
	protected override int DeserializeInner(ReadOnlySpan<byte> slice) => BinaryPrimitives.ReadInt32LittleEndian(slice);
}
