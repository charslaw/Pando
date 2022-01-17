using System;
using System.Buffers.Binary;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes/deserializes <c>ulong</c> values in little endian encoding (least significant byte first)
public class UInt64LittleEndianSerializer : FixedSizeBaseSerializer<ulong>
{
	/// <summary>A global default instance for <see cref="UInt64LittleEndianSerializer"/></summary>
	public static UInt64LittleEndianSerializer Default { get; } = new();

	public override int FixedSize => sizeof(ulong);
	protected override void SerializeInner(ulong value, Span<byte> slice) => BinaryPrimitives.WriteUInt64LittleEndian(slice, value);
	protected override ulong DeserializeInner(ReadOnlySpan<byte> slice) => BinaryPrimitives.ReadUInt64LittleEndian(slice);
}
