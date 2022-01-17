using System;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes/deserializes <c>bool</c> values as a single byte
public class BooleanSerializer : FixedSizeBaseSerializer<bool>
{
	/// <summary>A global default instance for <see cref="BooleanSerializer"/></summary>
	public static BooleanSerializer Default { get; } = new();

	public override int FixedSize => sizeof(byte);
	protected override void SerializeInner(bool value, Span<byte> slice) => slice[0] = (byte)(value ? 1 : 0);
	protected override bool DeserializeInner(ReadOnlySpan<byte> slice) => slice[0] != 0;
}
