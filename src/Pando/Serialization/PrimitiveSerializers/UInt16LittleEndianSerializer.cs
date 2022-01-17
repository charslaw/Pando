using System;
using System.Buffers.Binary;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes/deserializes <c>ushort</c> values in little endian encoding (least significant byte first)
public class UInt16LittleEndianSerializer : FixedSizeBaseSerializer<ushort>
{
	/// <summary>A global default instance for <see cref="UInt16LittleEndianSerializer"/></summary>
	public static UInt16LittleEndianSerializer Default { get; } = new();

	public override int FixedSize => sizeof(ushort);
	protected override void SerializeInner(ushort value, Span<byte> slice) => BinaryPrimitives.WriteUInt16LittleEndian(slice, value);
	protected override ushort DeserializeInner(ReadOnlySpan<byte> slice) => BinaryPrimitives.ReadUInt16LittleEndian(slice);
}
