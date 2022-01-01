using System;

namespace Pando.Serialization.PrimitiveSerializers;

public interface IPrimitiveSerializer<T>
{
	/// Serializes the given value into the given byte buffer
	public void Serialize(T value, ref Span<byte> buffer);

	/// Deserializes a value from the given byte buffer
	public T Deserialize(ReadOnlySpan<byte> buffer);
}
