using System;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes/deserializes <c>sbyte</c> values
public class SByteSerializer : FixedSizeBaseSerializer<sbyte>
{
	/// <summary>A global default instance for <see cref="SByteSerializer"/></summary>
	public static SByteSerializer Default { get; } = new();

	public override int FixedSize => sizeof(sbyte);
	protected override void SerializeInner(sbyte value, Span<byte> slice) => slice[0] = (byte)value;
	protected override sbyte DeserializeInner(ReadOnlySpan<byte> slice) => (sbyte)slice[0];
}
