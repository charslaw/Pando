using System;
using System.Buffers.Binary;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes/deserializes <c>uint</c> values in little endian encoding (least significant byte first)
public class UInt32LittleEndianSerializer : FixedSizeBaseSerializer<uint>
{
	/// <summary>A global default instance for <see cref="UInt32LittleEndianSerializer"/></summary>
	public static UInt32LittleEndianSerializer Default { get; } = new();

	protected override int FixedSize => sizeof(uint);
	protected override void SerializeInner(uint value, Span<byte> slice) => BinaryPrimitives.WriteUInt32LittleEndian(slice, value);
	protected override uint DeserializeInner(ReadOnlySpan<byte> slice) => BinaryPrimitives.ReadUInt32LittleEndian(slice);
}
