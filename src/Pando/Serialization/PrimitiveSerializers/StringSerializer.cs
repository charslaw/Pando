using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes a string encoded as ASCII
public class StringSerializer : IPrimitiveSerializer<string>
{
	private readonly IPrimitiveSerializer<int> _lengthSerializer;
	private readonly Encoding _encoding;

	public int? ByteCount => null;

	public int ByteCountForValue(string value)
	{
		var stringBytes = _encoding.GetByteCount(value);
		return stringBytes + ByteCountForLength(stringBytes);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int ByteCountForLength(int stringBytes) => _lengthSerializer.ByteCount ?? _lengthSerializer.ByteCountForValue(stringBytes);

	public StringSerializer(Encoding encoding) : this(Int32LittleEndianSerializer.Default, encoding) { }

	public StringSerializer(IPrimitiveSerializer<int> lengthSerializer, Encoding encoding)
	{
		_lengthSerializer = lengthSerializer;
		_encoding = encoding;
	}

	public void Serialize(string value, ref Span<byte> buffer)
	{
		var stringByteCount = _encoding.GetByteCount(value);
		var totalByteCount = ByteCountForLength(stringByteCount) + stringByteCount;

		if (buffer.Length < totalByteCount)
		{
			throw new ArgumentOutOfRangeException(nameof(buffer),
				"Buffer is not large enough to encode the given string." +
				$" Requires {totalByteCount} bytes, but buffer was only {buffer.Length} bytes in length."
			);
		}

		_lengthSerializer.Serialize(stringByteCount, ref buffer);
		_encoding.GetBytes(value, buffer);
		buffer = buffer[stringByteCount..];
	}

	public string Deserialize(ref ReadOnlySpan<byte> buffer)
	{
		var remainingBuffer = buffer;
		var stringByteCount = _lengthSerializer.Deserialize(ref remainingBuffer);

		if (remainingBuffer.Length < stringByteCount)
		{
			throw new ArgumentOutOfRangeException(nameof(buffer),
				$"Buffer is not large enough to decode a string with byte length {stringByteCount}." +
				$"Given buffer is {remainingBuffer.Length} bytes without including length bytes."
			);
		}

		var value = _encoding.GetString(remainingBuffer);
		buffer = remainingBuffer[stringByteCount..];

		return value;
	}
}
