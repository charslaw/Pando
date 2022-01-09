using System;
using System.Buffers.Binary;

namespace Pando.Serialization.PrimitiveSerializers;

public class DoubleLittleEndianSerializer : FixedSizeBaseSerializer<double>
{
	/// <summary>A global default instance for <see cref="SingleLittleEndianSerializer"/></summary>
	public static DoubleLittleEndianSerializer Default { get; } = new();

	protected override int FixedSize => sizeof(double);
	protected override void SerializeInner(double value, Span<byte> slice) => BinaryPrimitives.WriteDoubleLittleEndian(slice, value);
	protected override double DeserializeInner(ReadOnlySpan<byte> slice) => BinaryPrimitives.ReadDoubleLittleEndian(slice);
}
