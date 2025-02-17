using System;
using FluentAssertions;
using Pando.Serialization;
using Xunit;

namespace PandoTests.Tests.Serialization.Primitives;

/// A base class declaring a number of tests that all primitive serializers should pass
public class PrimitiveSerializerTest
{
	/// Used to overallocate serialization/deserialization buffers so that we can verify that the serializers chop off the appropriate number of bytes.
	private const int EXTRA_BUFFER_SPACE = 4;

	/// <summary>
	/// Verifies that <see cref="IPandoSerializer{T}.Serialize"/> writes the correct bytes to the given buffer.
	/// </summary>
	[Theory]
	[MemberData(nameof(BooleanSerializerTestData.SerializationTestData), MemberType = typeof(BooleanSerializerTestData))]
	[MemberData(nameof(EnumSerializerTestData.SerializationTestData), MemberType = typeof(EnumSerializerTestData))]
	[MemberData(nameof(SingleLittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(SingleLittleEndianSerializerTestData))]
	[MemberData(nameof(DoubleLittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(DoubleLittleEndianSerializerTestData))]
	[MemberData(nameof(SByteSerializerTestData.SerializationTestData), MemberType = typeof(SByteSerializerTestData))]
	[MemberData(nameof(ByteSerializerTestData.SerializationTestData), MemberType = typeof(ByteSerializerTestData))]
	[MemberData(nameof(Int16LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(Int16LittleEndianSerializerTestData))]
	[MemberData(nameof(UInt16LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(UInt16LittleEndianSerializerTestData))]
	[MemberData(nameof(Int32LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(Int32LittleEndianSerializerTestData))]
	[MemberData(nameof(UInt32LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(UInt32LittleEndianSerializerTestData))]
	[MemberData(nameof(Int64LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(Int64LittleEndianSerializerTestData))]
	[MemberData(nameof(UInt64LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(UInt64LittleEndianSerializerTestData))]
	[MemberData(nameof(DateTimeToBinarySerializerTestData.SerializationTestData), MemberType = typeof(DateTimeToBinarySerializerTestData))]
	[MemberData(nameof(TimeSpanTicksSerializerTestData.SerializationTestData), MemberType = typeof(TimeSpanTicksSerializerTestData))]
	[MemberData(nameof(DateOnlyDayNumberSerializerTestData.SerializationTestData), MemberType = typeof(DateOnlyDayNumberSerializerTestData))]
	[MemberData(nameof(TimeOnlyDayNumberSerializerTestData.SerializationTestData), MemberType = typeof(TimeOnlyDayNumberSerializerTestData))]
	public void Serialize_should_produce_correct_bytes<T>(T inputValue, byte[] expectedBytes, IPandoSerializer<T> serializer)
	{
		Span<byte> nodeBytes = stackalloc byte[expectedBytes.Length];
		var writeBuffer = nodeBytes;

		serializer.Serialize(inputValue, writeBuffer, null!);

		var serializationResult = nodeBytes[..expectedBytes.Length].ToArray();
		serializationResult.Should().BeEquivalentTo(expectedBytes);
	}

	/// <summary>
	/// Verifies that <see cref="IPandoSerializer{T}.Serialize"/> will throw an
	/// <see cref="ArgumentOutOfRangeException"/> if the given buffer is smaller than required for the given value.<br />
	/// </summary>
	[Theory]
	[MemberData(nameof(BooleanSerializerTestData.SerializationTestData), MemberType = typeof(BooleanSerializerTestData))]
	[MemberData(nameof(EnumSerializerTestData.SerializationTestData), MemberType = typeof(EnumSerializerTestData))]
	[MemberData(nameof(SingleLittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(SingleLittleEndianSerializerTestData))]
	[MemberData(nameof(DoubleLittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(DoubleLittleEndianSerializerTestData))]
	[MemberData(nameof(SByteSerializerTestData.SerializationTestData), MemberType = typeof(SByteSerializerTestData))]
	[MemberData(nameof(ByteSerializerTestData.SerializationTestData), MemberType = typeof(ByteSerializerTestData))]
	[MemberData(nameof(Int16LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(Int16LittleEndianSerializerTestData))]
	[MemberData(nameof(UInt16LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(UInt16LittleEndianSerializerTestData))]
	[MemberData(nameof(Int32LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(Int32LittleEndianSerializerTestData))]
	[MemberData(nameof(UInt32LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(UInt32LittleEndianSerializerTestData))]
	[MemberData(nameof(Int64LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(Int64LittleEndianSerializerTestData))]
	[MemberData(nameof(UInt64LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(UInt64LittleEndianSerializerTestData))]
	[MemberData(nameof(DateTimeToBinarySerializerTestData.SerializationTestData), MemberType = typeof(DateTimeToBinarySerializerTestData))]
	[MemberData(nameof(TimeSpanTicksSerializerTestData.SerializationTestData), MemberType = typeof(TimeSpanTicksSerializerTestData))]
	[MemberData(nameof(DateOnlyDayNumberSerializerTestData.SerializationTestData), MemberType = typeof(DateOnlyDayNumberSerializerTestData))]
	[MemberData(nameof(TimeOnlyDayNumberSerializerTestData.SerializationTestData), MemberType = typeof(TimeOnlyDayNumberSerializerTestData))]
	public void Serialize_should_throw_when_buffer_is_too_small<T>(T inputValue, byte[] expectedBytes, IPandoSerializer<T> serializer)
	{
		serializer.Invoking(s =>
				{
					Span<byte> undersizedBuffer = new byte[expectedBytes.Length - 1];
					s.Serialize(inputValue, undersizedBuffer, null!);
				}
			)
			.Should()
			.Throw<ArgumentOutOfRangeException>();
	}

	/// <summary>
	/// Verifies that <see cref="IPandoSerializer{T}.Deserialize"/> reads the correct value from the given buffer.
	/// </summary>
	[Theory]
	[MemberData(nameof(BooleanSerializerTestData.SerializationTestData), MemberType = typeof(BooleanSerializerTestData))]
	[MemberData(nameof(EnumSerializerTestData.SerializationTestData), MemberType = typeof(EnumSerializerTestData))]
	[MemberData(nameof(SingleLittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(SingleLittleEndianSerializerTestData))]
	[MemberData(nameof(DoubleLittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(DoubleLittleEndianSerializerTestData))]
	[MemberData(nameof(SByteSerializerTestData.SerializationTestData), MemberType = typeof(SByteSerializerTestData))]
	[MemberData(nameof(ByteSerializerTestData.SerializationTestData), MemberType = typeof(ByteSerializerTestData))]
	[MemberData(nameof(Int16LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(Int16LittleEndianSerializerTestData))]
	[MemberData(nameof(UInt16LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(UInt16LittleEndianSerializerTestData))]
	[MemberData(nameof(Int32LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(Int32LittleEndianSerializerTestData))]
	[MemberData(nameof(UInt32LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(UInt32LittleEndianSerializerTestData))]
	[MemberData(nameof(Int64LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(Int64LittleEndianSerializerTestData))]
	[MemberData(nameof(UInt64LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(UInt64LittleEndianSerializerTestData))]
	[MemberData(nameof(DateTimeToBinarySerializerTestData.SerializationTestData), MemberType = typeof(DateTimeToBinarySerializerTestData))]
	[MemberData(nameof(TimeSpanTicksSerializerTestData.SerializationTestData), MemberType = typeof(TimeSpanTicksSerializerTestData))]
	[MemberData(nameof(DateOnlyDayNumberSerializerTestData.SerializationTestData), MemberType = typeof(DateOnlyDayNumberSerializerTestData))]
	[MemberData(nameof(TimeOnlyDayNumberSerializerTestData.SerializationTestData), MemberType = typeof(TimeOnlyDayNumberSerializerTestData))]
	public void Deserialize_should_produce_correct_value<T>(T expectedValue, byte[] inputBytes, IPandoSerializer<T> serializer)
	{
		// Over-allocate read buffer to ensure that the serializer doesn't get greedy with reading from the read buffer
		// If the serializer reads into the extra buffer space, presumably it will not produce the correct result.
		Span<byte> nodeBytes = new byte[inputBytes.Length + EXTRA_BUFFER_SPACE];
		inputBytes.CopyTo(nodeBytes);

		var deserializationResult = serializer.Deserialize(nodeBytes, null!);

		deserializationResult.Should().Be(expectedValue);
	}

	/// <summary>
	/// Verifies that <see cref="IPandoSerializer{T}.Deserialize"/> reads the correct value from the given buffer.
	/// </summary>
	[Theory]
	[MemberData(nameof(DateTimeToBinarySerializerTestData.SerializationTestData), MemberType = typeof(DateTimeToBinarySerializerTestData))]
	public void Deserialize_should_produce_correct_DateTime_value(DateTime expectedValue, byte[] inputBytes, IPandoSerializer<DateTime> serializer)
	{
		// Over-allocate read buffer to ensure that the serializer doesn't get greedy with reading from the read buffer
		// If the serializer reads into the extra buffer space, presumably it will not produce the correct result.
		Span<byte> nodeBytes = new byte[inputBytes.Length + EXTRA_BUFFER_SPACE];
		inputBytes.CopyTo(nodeBytes);

		var deserializationResult = serializer.Deserialize(nodeBytes, null!);

		deserializationResult.Should().Be(expectedValue);
		deserializationResult.Kind.Should().Be(expectedValue.Kind);
	}

#pragma warning disable xUnit1026 // ignore unused parameter warning; we want to use the same data for multiple tests

	/// <summary>
	/// Verifies that <see cref="IPandoSerializer{T}.Deserialize"/> will throw an
	/// <see cref="ArgumentOutOfRangeException"/> if the given buffer is too small to read a value from.
	/// </summary>
	/// <remarks>
	/// This is trivial for fixed size serializers, but for variable size serializers, whether or not the buffer is
	/// undersized can depend on the contents of the buffer. For these cases the serializer must read from the buffer,
	/// but must not chop the buffer in the case that the buffer is an incorrect size.
	/// </remarks>
	[Theory]
	[MemberData(nameof(BooleanSerializerTestData.SerializationTestData), MemberType = typeof(BooleanSerializerTestData))]
	[MemberData(nameof(EnumSerializerTestData.SerializationTestData), MemberType = typeof(EnumSerializerTestData))]
	[MemberData(nameof(SingleLittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(SingleLittleEndianSerializerTestData))]
	[MemberData(nameof(DoubleLittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(DoubleLittleEndianSerializerTestData))]
	[MemberData(nameof(SByteSerializerTestData.SerializationTestData), MemberType = typeof(SByteSerializerTestData))]
	[MemberData(nameof(ByteSerializerTestData.SerializationTestData), MemberType = typeof(ByteSerializerTestData))]
	[MemberData(nameof(Int16LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(Int16LittleEndianSerializerTestData))]
	[MemberData(nameof(UInt16LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(UInt16LittleEndianSerializerTestData))]
	[MemberData(nameof(Int32LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(Int32LittleEndianSerializerTestData))]
	[MemberData(nameof(UInt32LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(UInt32LittleEndianSerializerTestData))]
	[MemberData(nameof(Int64LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(Int64LittleEndianSerializerTestData))]
	[MemberData(nameof(UInt64LittleEndianSerializerTestData.SerializationTestData), MemberType = typeof(UInt64LittleEndianSerializerTestData))]
	[MemberData(nameof(DateTimeToBinarySerializerTestData.SerializationTestData), MemberType = typeof(DateTimeToBinarySerializerTestData))]
	[MemberData(nameof(TimeSpanTicksSerializerTestData.SerializationTestData), MemberType = typeof(TimeSpanTicksSerializerTestData))]
	[MemberData(nameof(DateOnlyDayNumberSerializerTestData.SerializationTestData), MemberType = typeof(DateOnlyDayNumberSerializerTestData))]
	[MemberData(nameof(TimeOnlyDayNumberSerializerTestData.SerializationTestData), MemberType = typeof(TimeOnlyDayNumberSerializerTestData))]
	public void Deserialize_should_throw_when_buffer_is_too_small<T>(T _, byte[] inputBytes, IPandoSerializer<T> serializer)
	{
		serializer.Invoking(s =>
				{
					ReadOnlySpan<byte> undersizedBuffer = inputBytes.AsSpan(..^1);
					s.Deserialize(undersizedBuffer, null!);
				}
			)
			.Should()
			.Throw<ArgumentOutOfRangeException>();
	}

#pragma warning restore xUnit1026 // unused parameter warning
}
