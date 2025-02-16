using System;
using FluentAssertions;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

/// A base class declaring a number of tests that all primitive serializers should pass
public class PrimitiveSerializerTest
{
	/// Used to overallocate serialization/deserialization buffers so that we can verify that the serializers chop off the appropriate number of bytes.
	private const int EXTRA_BUFFER_SPACE = 4;

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.Serialize"/> writes the correct bytes to the given buffer.
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
	public void Serialize_should_produce_correct_bytes<T>(T inputValue, byte[] expectedBytes, IPrimitiveSerializer<T> serializer)
	{
		Span<byte> nodeBytes = stackalloc byte[expectedBytes.Length];
		var writeBuffer = nodeBytes;

		serializer.Serialize(inputValue, ref writeBuffer);

		var serializationResult = nodeBytes[..expectedBytes.Length].ToArray();
		serializationResult.Should().BeEquivalentTo(expectedBytes);
	}

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.Serialize"/> will chop the write buffer to the slice remaining after the write.
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
	public void Serialize_should_chop_used_bytes_from_buffer<T>(T inputValue, byte[] expectedBytes, IPrimitiveSerializer<T> serializer)
	{
		Span<byte> nodeBytes = stackalloc byte[expectedBytes.Length + EXTRA_BUFFER_SPACE];
		var writeBuffer = nodeBytes;
		serializer.Serialize(inputValue, ref writeBuffer);
		writeBuffer.Length.Should().Be(EXTRA_BUFFER_SPACE);
	}

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.Serialize"/> will throw an
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
	public void Serialize_should_throw_when_buffer_is_too_small<T>(T inputValue, byte[] expectedBytes, IPrimitiveSerializer<T> serializer)
	{
		serializer.Invoking(s =>
				{
					Span<byte> undersizedBuffer = new byte[expectedBytes.Length - 1];
					s.Serialize(inputValue, ref undersizedBuffer);
				}
			)
			.Should()
			.Throw<ArgumentOutOfRangeException>();
	}

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.Serialize"/> will not alter
	/// the given write buffer if it is too small to write a value to.
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
	public void Serialize_should_not_alter_size_of_buffer_when_it_is_too_small<T>(T inputValue, byte[] expectedBytes, IPrimitiveSerializer<T> serializer)
	{
		var beforeBufferSize = expectedBytes.Length - 1;
		Span<byte> undersizedBuffer = stackalloc byte[beforeBufferSize];

		try { serializer.Serialize(inputValue, ref undersizedBuffer); }
		catch (ArgumentOutOfRangeException) { } // this will throw but we don't care about the exception, we just want to test a post-condition

		undersizedBuffer.Length.Should().Be(beforeBufferSize);
	}

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.Deserialize"/> reads the correct value from the given buffer.
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
	public void Deserialize_should_produce_correct_value<T>(T expectedValue, byte[] inputBytes, IPrimitiveSerializer<T> serializer)
	{
		// Over-allocate read buffer to ensure that the serializer doesn't get greedy with reading from the read buffer
		// If the serializer reads into the extra buffer space, presumably it will not produce the correct result.
		Span<byte> nodeBytes = new byte[inputBytes.Length + EXTRA_BUFFER_SPACE];
		inputBytes.CopyTo(nodeBytes);
		ReadOnlySpan<byte> readBuffer = nodeBytes;

		var deserializationResult = serializer.Deserialize(ref readBuffer);

		deserializationResult.Should().Be(expectedValue);
	}
	
	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.Deserialize"/> reads the correct value from the given buffer.
	/// </summary>
	[Theory]
	[MemberData(nameof(DateTimeToBinarySerializerTestData.SerializationTestData), MemberType = typeof(DateTimeToBinarySerializerTestData))]
	public void Deserialize_should_produce_correct_DateTime_value(DateTime expectedValue, byte[] inputBytes, IPrimitiveSerializer<DateTime> serializer)
	{
		// Over-allocate read buffer to ensure that the serializer doesn't get greedy with reading from the read buffer
		// If the serializer reads into the extra buffer space, presumably it will not produce the correct result.
		Span<byte> nodeBytes = new byte[inputBytes.Length + EXTRA_BUFFER_SPACE];
		inputBytes.CopyTo(nodeBytes);
		ReadOnlySpan<byte> readBuffer = nodeBytes;
	
		var deserializationResult = serializer.Deserialize(ref readBuffer);
		deserializationResult.Should().Be(expectedValue);
		deserializationResult.Kind.Should().Be(expectedValue.Kind);
	}

#pragma warning disable xUnit1026 // ignore unused parameter warning; we want to use the same data for multiple tests

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.Deserialize"/> will chop the read buffer to the slice remaining after the read.
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
	public void Deserialize_should_chop_used_bytes_from_buffer<T>(T _, byte[] inputBytes, IPrimitiveSerializer<T> serializer)
	{
		Span<byte> oversizedBuffer = stackalloc byte[inputBytes.Length + EXTRA_BUFFER_SPACE];
		inputBytes.CopyTo(oversizedBuffer);
		ReadOnlySpan<byte> readBuffer = oversizedBuffer;
		var __ = serializer.Deserialize(ref readBuffer);
		readBuffer.Length.Should().Be(EXTRA_BUFFER_SPACE);
	}

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.Deserialize"/> will throw an
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
	public void Deserialize_should_throw_when_buffer_is_too_small<T>(T _, byte[] inputBytes, IPrimitiveSerializer<T> serializer)
	{
		serializer.Invoking(s =>
				{
					ReadOnlySpan<byte> undersizedBuffer = inputBytes.AsSpan(..^1);
					s.Deserialize(ref undersizedBuffer);
				}
			)
			.Should()
			.Throw<ArgumentOutOfRangeException>();
	}

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.Deserialize"/> will not alter
	/// the given read buffer if it is too small to read a value from.
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
	public void Deserialize_should_not_alter_size_of_buffer_when_it_is_too_small<T>(T _, byte[] inputBytes, IPrimitiveSerializer<T> serializer)
	{
		var beforeBufferSize = inputBytes.Length - 1;
		ReadOnlySpan<byte> undersizedBuffer = inputBytes.AsSpan(..^1);

		try { serializer.Deserialize(ref undersizedBuffer); }
		catch (ArgumentOutOfRangeException) { } // this will throw but we don't care about the exception, we just want to test a post-condition

		undersizedBuffer.Length.Should().Be(beforeBufferSize);
	}

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.ByteCount"/> returns the appropriate size.
	/// </summary>
	/// T needs to be passed here so that the generic argument can be resolved by xunit
	[Theory]
	[MemberData(nameof(BooleanSerializerTestData.ByteCountTestData), MemberType = typeof(BooleanSerializerTestData))]
	[MemberData(nameof(EnumSerializerTestData.ByteCountTestData), MemberType = typeof(EnumSerializerTestData))]
	[MemberData(nameof(SingleLittleEndianSerializerTestData.ByteCountTestData), MemberType = typeof(SingleLittleEndianSerializerTestData))]
	[MemberData(nameof(DoubleLittleEndianSerializerTestData.ByteCountTestData), MemberType = typeof(DoubleLittleEndianSerializerTestData))]
	[MemberData(nameof(SByteSerializerTestData.ByteCountTestData), MemberType = typeof(SByteSerializerTestData))]
	[MemberData(nameof(ByteSerializerTestData.ByteCountTestData), MemberType = typeof(ByteSerializerTestData))]
	[MemberData(nameof(Int16LittleEndianSerializerTestData.ByteCountTestData), MemberType = typeof(Int16LittleEndianSerializerTestData))]
	[MemberData(nameof(UInt16LittleEndianSerializerTestData.ByteCountTestData), MemberType = typeof(UInt16LittleEndianSerializerTestData))]
	[MemberData(nameof(Int32LittleEndianSerializerTestData.ByteCountTestData), MemberType = typeof(Int32LittleEndianSerializerTestData))]
	[MemberData(nameof(UInt32LittleEndianSerializerTestData.ByteCountTestData), MemberType = typeof(UInt32LittleEndianSerializerTestData))]
	[MemberData(nameof(Int64LittleEndianSerializerTestData.ByteCountTestData), MemberType = typeof(Int64LittleEndianSerializerTestData))]
	[MemberData(nameof(UInt64LittleEndianSerializerTestData.ByteCountTestData), MemberType = typeof(UInt64LittleEndianSerializerTestData))]
	[MemberData(nameof(DateTimeToBinarySerializerTestData.ByteCountTestData), MemberType = typeof(DateTimeToBinarySerializerTestData))]
	[MemberData(nameof(TimeSpanTicksSerializerTestData.ByteCountTestData), MemberType = typeof(TimeSpanTicksSerializerTestData))]
	[MemberData(nameof(DateOnlyDayNumberSerializerTestData.ByteCountTestData), MemberType = typeof(DateOnlyDayNumberSerializerTestData))]
	[MemberData(nameof(TimeOnlyDayNumberSerializerTestData.ByteCountTestData), MemberType = typeof(TimeOnlyDayNumberSerializerTestData))]
	public void ByteCount_should_return_correct_size<T>(T _, int? expectedSize, IPrimitiveSerializer<T> serializer)
	{
		serializer.ByteCount.Should().Be(expectedSize);
	}

#pragma warning restore xUnit1026 // unused parameter warning

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.ByteCountForValue"/> returns the appropriate size for the given value.
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
	public void ByteCountForValue_should_return_correct_size<T>(T value, byte[] expectedBytes, IPrimitiveSerializer<T> serializer)
	{
		serializer.ByteCountForValue(value).Should().Be(expectedBytes.Length);
	}
}
