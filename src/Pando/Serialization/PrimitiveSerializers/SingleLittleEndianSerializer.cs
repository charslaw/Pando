using System;
using System.Buffers.Binary;

namespace Pando.Serialization.PrimitiveSerializers;

public class SingleLittleEndianSerializer : FixedSizeBaseSerializer<float>
{
	/// <summary>A global default instance for <see cref="SingleLittleEndianSerializer"/></summary>
	public static SingleLittleEndianSerializer Default { get; } = new();

	protected override int FixedSize => sizeof(float);
	protected override void SerializeInner(float value, Span<byte> slice) => BinaryPrimitives.WriteSingleLittleEndian(slice, value);
	protected override float DeserializeInner(ReadOnlySpan<byte> slice) => BinaryPrimitives.ReadSingleLittleEndian(slice);
}
