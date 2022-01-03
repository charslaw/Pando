using System;

namespace Pando.Serialization.PrimitiveSerializers;

public interface IPrimitiveSerializer<T>
{
	/// The size in bytes of the data produced by this serializer, if known.
	/// If the size can be dynamic, should return null.
	/// <remarks>This should be constant after initialization. If the size of the data can vary, return null</remarks>
	public int? ByteCount { get; }

	/// The size in bytes of the data that would be produced by this serializer based on the given concrete value.
	/// This should always be possible to calculate for a given input, so it is not nullable like <see cref="ByteCount"/>.
	public int ByteCountForValue(T value);

	/// Serializes the given value into the given byte buffer
	public void Serialize(T value, Span<byte> buffer);

	/// Deserializes a value from the given byte buffer
	public T Deserialize(ReadOnlySpan<byte> buffer);
}
