using System;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes/deserializes single <c>byte</c> values
public class ByteSerializer : FixedSizeBaseSerializer<byte>
{
	/// <summary>A global default instance for <see cref="ByteSerializer"/></summary>
	public static ByteSerializer Default { get; } = new();

	protected override int FixedSize => sizeof(byte);
	protected override void SerializeInner(byte value, Span<byte> slice) => slice[0] = value;
	protected override byte DeserializeInner(ReadOnlySpan<byte> slice) => slice[0];
}
