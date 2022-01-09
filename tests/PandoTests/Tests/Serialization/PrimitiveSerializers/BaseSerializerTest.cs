using System;
using FluentAssertions;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

/// A base class declaring a number of tests that all primitive serializers should pass
public abstract class BaseSerializerTest<T>
{
	/// Used to overallocate serialization/deserialization buffers so that we can verify that the serializers chop off the appropriate number of bytes.
	private const int EXTRA_BUFFER_SPACE = 1;

	/// Provides the serializer under test
	protected abstract IPrimitiveSerializer<T> Serializer { get; }

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.Serialize"/> writes the correct bytes to the given buffer.
	/// </summary>
	[Theory]
	[MemberData(nameof(ISerializerTestData<T>.SerializationTestData))]
	public virtual void Serialize_should_produce_correct_bytes(T inputValue, byte[] expectedBytes)
	{
		Span<byte> nodeBytes = stackalloc byte[expectedBytes.Length];
		var writeBuffer = nodeBytes;

		Serializer.Serialize(inputValue, ref writeBuffer);

		var serializationResult = nodeBytes[..expectedBytes.Length].ToArray();
		serializationResult.Should().BeEquivalentTo(expectedBytes);
	}

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.Serialize"/> will chop the write buffer to the slice remaining after the write.
	/// </summary>
	[Theory]
	[MemberData(nameof(ISerializerTestData<T>.SerializationTestData))]
	public virtual void Serialize_should_chop_used_bytes_from_buffer(T inputValue, byte[] expectedBytes)
	{
		Span<byte> nodeBytes = stackalloc byte[expectedBytes.Length + EXTRA_BUFFER_SPACE];
		var writeBuffer = nodeBytes;
		Serializer.Serialize(inputValue, ref writeBuffer);
		writeBuffer.Length.Should().Be(EXTRA_BUFFER_SPACE);
	}

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.Serialize"/> will throw an
	/// <see cref="ArgumentOutOfRangeException"/> if the given buffer is smaller than required for the given value.<br />
	/// </summary>
	[Theory]
	[MemberData(nameof(ISerializerTestData<T>.SerializationTestData))]
	public virtual void Serialize_should_throw_when_buffer_is_too_small(T inputValue, byte[] expectedBytes)
	{
		Serializer.Invoking(s =>
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
	[MemberData(nameof(ISerializerTestData<T>.SerializationTestData))]
	public virtual void Serialize_should_not_alter_size_of_buffer_when_it_is_too_small(T inputValue, byte[] expectedBytes)
	{
		var beforeBufferSize = expectedBytes.Length - 1;
		Span<byte> undersizedBuffer = stackalloc byte[beforeBufferSize];

		try { Serializer.Serialize(inputValue, ref undersizedBuffer); }
		catch (ArgumentOutOfRangeException) { } // this will throw but we don't care about the exception, we just want to test a post-condition

		undersizedBuffer.Length.Should().Be(beforeBufferSize);
	}

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.Deserialize"/> reads the correct value from the given buffer.
	/// </summary>
	[Theory]
	[MemberData(nameof(ISerializerTestData<T>.SerializationTestData))]
	public virtual void Deserialize_should_produce_correct_value(T expectedValue, byte[] inputBytes)
	{
		Span<byte> nodeBytes = new byte[inputBytes.Length];
		inputBytes.CopyTo(nodeBytes);
		ReadOnlySpan<byte> readBuffer = nodeBytes;

		var deserializationResult = Serializer.Deserialize(ref readBuffer);

		deserializationResult.Should().BeEquivalentTo(expectedValue);
	}

#pragma warning disable xUnit1026 // ignore unused parameter warning; we want to use the same data for multiple tests

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.Deserialize"/> will chop the read buffer to the slice remaining after the read.
	/// </summary>
	[Theory]
	[MemberData(nameof(ISerializerTestData<T>.SerializationTestData))]
	public virtual void Deserialize_should_chop_used_bytes_from_buffer(T _, byte[] inputBytes)
	{
		Span<byte> oversizedBuffer = stackalloc byte[inputBytes.Length + EXTRA_BUFFER_SPACE];
		inputBytes.CopyTo(oversizedBuffer);
		ReadOnlySpan<byte> readBuffer = oversizedBuffer;
		var __ = Serializer.Deserialize(ref readBuffer);
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
	[MemberData(nameof(ISerializerTestData<T>.SerializationTestData))]
	public virtual void Deserialize_should_throw_when_buffer_is_too_small(T _, byte[] inputBytes)
	{
		Serializer.Invoking(s =>
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
	[MemberData(nameof(ISerializerTestData<T>.SerializationTestData))]
	public virtual void Deserialize_should_not_alter_size_of_buffer_when_it_is_too_small(T _, byte[] inputBytes)
	{
		var beforeBufferSize = inputBytes.Length - 1;
		ReadOnlySpan<byte> undersizedBuffer = inputBytes.AsSpan(..^1);

		try { Serializer.Deserialize(ref undersizedBuffer); }
		catch (ArgumentOutOfRangeException) { } // this will throw but we don't care about the exception, we just want to test a post-condition

		undersizedBuffer.Length.Should().Be(beforeBufferSize);
	}

#pragma warning restore xUnit1026 // unused parameter warning

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.ByteCount"/> returns the appropriate size.
	/// </summary>
	[Theory]
	[MemberData(nameof(ISerializerTestData<T>.ByteCountTestData))]
	public virtual void ByteCount_should_return_correct_size(int? expectedSize)
	{
		Serializer.ByteCount.Should().Be(expectedSize);
	}

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.ByteCountForValue"/> returns the appropriate size for the given value.
	/// </summary>
	[Theory]
	[MemberData(nameof(ISerializerTestData<T>.SerializationTestData))]
	public virtual void ByteCountForValue_should_return_correct_size(T value, byte[] expectedBytes)
	{
		Serializer.ByteCountForValue(value).Should().Be(expectedBytes.Length);
	}
}

/// Defines the test data providers required by BaseSerializerTest. This should be implemented by any subclass of BaseSerializerTest
///
/// <remarks>Note that BaseSerializerTest itself can't implement this interface because it would need to implement these methods and it can't
/// keep them abstract because abstract static is *only* supported in interfaces.</remarks>
public interface ISerializerTestData<T>
{
	/// <summary>Defines valid pairs of value and serialized bytes.</summary>
	/// <remarks>This is re-used for many tests in various forms.
	/// Since each entry should represent a valid usage of both Serialize and Deserialize,
	/// we can mutate the data to produce invalid data to test error cases.</remarks>
	public abstract static TheoryData<T, byte[]> SerializationTestData { get; }

	/// Defines the expected result of the ByteCount property
	public abstract static TheoryData<int?> ByteCountTestData { get; }
}
