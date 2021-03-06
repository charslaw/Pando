using System;

namespace Pando.Serialization.PrimitiveSerializers;

/// <summary>
/// Serializes/deserializes <see cref="Nullable{T}"/> values via a provided inner serializer.
/// </summary>
/// <remarks>
/// This serializer produces variable-width output.
/// The first byte of the serialized output encodes whether the value is null or not.
/// If the value is null, it takes up a single byte.
/// If the value is not null, it takes up N+1 bytes, where N is the number of bytes produced by the inner serializer.
/// </remarks>
public class NullableSerializer<T> : IPrimitiveSerializer<T?>
	where T : struct
{
	private readonly IPrimitiveSerializer<T> _innerSerializer;

	/// The number of bytes used to store the null state of the value
	private const int NULL_FLAG_SIZE = 1;

	/// <summary>Creates a new <see cref="NullableSerializer{T}"/> using the given
	/// <paramref name="innerSerializer"/> to serialize non-null values.</summary>
	public NullableSerializer(IPrimitiveSerializer<T> innerSerializer)
	{
		_innerSerializer = innerSerializer;
	}

	// Size is variable because if the value is null then we just store an empty byte.
	public int? ByteCount => null;

	public int ByteCountForValue(T? value) =>
		value is not null
			? NULL_FLAG_SIZE + (_innerSerializer.ByteCount ?? _innerSerializer.ByteCountForValue(value.Value))
			: NULL_FLAG_SIZE; // just a single empty byte in the null case

	public void Serialize(T? value, ref Span<byte> buffer)
	{
		// Defensively take a copy of the bytes after removing the null flag byte.
		// If the inner serializer throws, the given buffer will remain unmodified.
		var remainingBuffer = buffer[1..];

		if (value is not null)
		{
			buffer[0] = 1;
			_innerSerializer.Serialize(value.Value, ref remainingBuffer);
		}
		else
		{
			buffer[0] = 0;
		}

		// If we got this far, the serialization was successful, so set the buffer accordingly.
		buffer = remainingBuffer;
	}

	public T? Deserialize(ref ReadOnlySpan<byte> buffer)
	{
		// Defensively take a copy of the bytes after removing the null flag byte.
		// If the inner serializer throws, the given buffer will remain unmodified.
		var remainingBuffer = buffer[1..];

		T? value = buffer[0] == 0 ? null : _innerSerializer.Deserialize(ref remainingBuffer);

		// If we got this far, the deserialization was successful, so set the buffer accordingly.
		buffer = remainingBuffer;

		return value;
	}
}
