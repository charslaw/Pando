using System;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes and deserializes a single "primitive" data type to and from raw bytes.
/// <remarks>A primitive data type -- for the purposes of this type of serializer -- is an atomic value:
/// a value which cannot be broken into smaller pieces, or has no significant meaning when broken down further.
/// This definition is not incredibly well defined or strict, but rather a general guideline.</remarks>
public interface IPrimitiveSerializer<T>
{
	/// The size in bytes of the data produced by this serializer, or null if the size can be dynamic.
	/// <remarks>This should be constant after the serializer has been initialized.
	/// If the size of the data can vary, return null.</remarks>
	public int? ByteCount { get; }

	/// The size in bytes of the data that would be produced by this serializer for the given concrete value.
	/// <remarks>This should always be possible to calculate for a given input,
	/// so it is not nullable like <see cref="ByteCount"/>.</remarks>
	public int ByteCountForValue(T value);

	/// Serializes the given value into the start of the given byte buffer,
	/// then slices the buffer to the remaining space after the write.
	/// <exception cref="ArgumentOutOfRangeException">thrown when the given <paramref name="buffer"/>
	/// is too small to contain the serialized encoding of the given value.</exception>
	public void Serialize(T value, ref Span<byte> buffer);

	/// Deserializes a value from the given byte buffer, then slices the buffer to the remaining unread space
	/// <exception cref="ArgumentOutOfRangeException">thrown when the given <paramref name="buffer"/>
	/// is too small to read a value of type <typeparamref name="T"/> from.</exception>
	public T Deserialize(ref ReadOnlySpan<byte> buffer);
}
