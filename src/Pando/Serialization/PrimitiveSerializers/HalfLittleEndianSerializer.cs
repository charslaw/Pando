using System;
using System.Buffers.Binary;

namespace Pando.Serialization.PrimitiveSerializers;

public class HalfLittleEndianSerializer : FixedSizeBaseSerializer<Half>
{
	/// <summary>A global default instance for <see cref="SingleLittleEndianSerializer"/></summary>
	public static HalfLittleEndianSerializer Default { get; } = new();

	public override unsafe int FixedSize => sizeof(Half);
	protected override void SerializeInner(Half value, Span<byte> slice) => BinaryPrimitives.WriteHalfLittleEndian(slice, value);
	protected override Half DeserializeInner(ReadOnlySpan<byte> slice) => BinaryPrimitives.ReadHalfLittleEndian(slice);
}
