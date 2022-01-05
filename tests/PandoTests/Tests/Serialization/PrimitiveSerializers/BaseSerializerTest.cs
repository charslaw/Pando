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
	/// <para>Verifies the following about <see cref="IPrimitiveSerializer{T}.Serialize"/>:</para>
	///   1. That it writes the correct bytes to the given buffer.<br />
	///   2. That it appropriately chops the write span to the remaining space after the write.
	/// </summary>
	[Theory]
	[MemberData(nameof(TestDataProviderPlaceholders.SerializationTestData))]
	public virtual void Serialize_should_produce_correct_bytes(T value, byte[] bytes)
	{
		Span<byte> nodeBytes = stackalloc byte[bytes.Length + EXTRA_BUFFER_SPACE];
		var writeBuffer = nodeBytes;

		Serializer.Serialize(value, ref writeBuffer);

		var serializationResult = nodeBytes[..bytes.Length].ToArray();
		serializationResult.Should().BeEquivalentTo(bytes);
		writeBuffer.Length.Should()
			.Be(EXTRA_BUFFER_SPACE,
				"because the serializer is expected to chop off the written-to slice from the write buffer"
			);
	}

	/// <summary>
	/// <para>Verifies the following about <see cref="IPrimitiveSerializer{T}.Deserialize"/>:</para>
	///   1. That it reads the correct value from the given buffer.<br />
	///   2. That it appropriately chops the read span to the remaining unread space after the read.
	/// </summary>
	[Theory]
	[MemberData(nameof(TestDataProviderPlaceholders.SerializationTestData))]
	public virtual void Deserialize_should_produce_correct_value(T value, byte[] bytes)
	{
		Span<byte> nodeBytes = new byte[bytes.Length + EXTRA_BUFFER_SPACE];
		bytes.CopyTo(nodeBytes);
		ReadOnlySpan<byte> readBuffer = nodeBytes;

		var deserializationResult = Serializer.Deserialize(ref readBuffer);

		deserializationResult.Should().BeEquivalentTo(value);
		readBuffer.Length.Should()
			.Be(EXTRA_BUFFER_SPACE,
				"because the serializer is expected to chop off the read-from slice from the read buffer"
			);
	}

	/// <summary>
	/// <para>Verifies the following about <see cref="IPrimitiveSerializer{T}.Serialize"/>:</para>
	///   1. That it will throw an <see cref="ArgumentOutOfRangeException"/> if the given buffer is smaller
	///			than required for the given value.<br />
	///   2. That when the buffer is too small, it does not resize the given span.
	/// </summary>
	[Theory]
	[MemberData(nameof(TestDataProviderPlaceholders.SerializeUndersizedBufferTestData))]
	public virtual void Serialize_should_throw_when_buffer_is_too_small(T value, int expectedSize)
	{
		var beforeBufferSize = expectedSize - 1;
		int? afterBufferSize = null;

		Serializer.Invoking(s =>
				{
					Span<byte> undersizedBuffer = new byte[beforeBufferSize];
					try
					{
						s.Serialize(value, ref undersizedBuffer);
					}
					finally
					{
						afterBufferSize = undersizedBuffer.Length;
					}
				}
			)
			.Should()
			.Throw<ArgumentOutOfRangeException>();

		afterBufferSize.Should()
			.Be(beforeBufferSize,
				"because when the buffer is too small, the serializer should not modify the passed in buffer reference"
			);
	}

	/// <summary>
	/// <para>Verifies the following about <see cref="IPrimitiveSerializer{T}.Deserialize"/>:</para>
	///   1. That it will throw an <see cref="ArgumentOutOfRangeException"/> if the given buffer is smaller
	///			than the size of the deserialized type.<br />
	///   2. That when the buffer is too small, it does not resize the given span.
	/// </summary>
	/// <remarks>
	/// This is trivial for fixed size serializers, but for variable size serializers, whether or not the buffer is
	/// undersized can depend on the contents of the buffer. For these cases the serializer must read from the buffer,
	/// but must not chop the buffer in the case that the buffer is an incorrect size.
	/// </remarks>
	[Theory]
	[MemberData(nameof(TestDataProviderPlaceholders.DeserializeUndersizedBufferTestData))]
	public virtual void Deserialize_should_throw_when_buffer_is_too_small(byte[] undersizedBuffer)
	{
		var beforeBufferSize = undersizedBuffer.Length;
		int? afterBufferSize = null;

		Serializer.Invoking(s =>
				{
					var bufferSpan = new ReadOnlySpan<byte>(undersizedBuffer);

					try
					{
						s.Deserialize(ref bufferSpan);
					}
					finally
					{
						afterBufferSize = bufferSpan.Length;
					}
				}
			)
			.Should()
			.Throw<ArgumentOutOfRangeException>();

		afterBufferSize.Should()
			.Be(beforeBufferSize,
				"because when the buffer is too small, the serializer should not modify the passed in buffer reference"
			);
	}

	/// <summary>
	/// Verifies that <see cref="IPrimitiveSerializer{T}.ByteCount"/> returns the appropriate value.
	/// </summary>
	[Theory]
	[MemberData(nameof(TestDataProviderPlaceholders.ByteCountTestData))]
	public virtual void ByteCount_should_return_correct_size(int? expectedSize)
	{
		Serializer.ByteCount.Should().Be(expectedSize);
	}
}

internal static class TestDataProviderPlaceholders
{
	public static void SerializationTestData() =>
		throw new NotImplementedException("This is just a placeholder for the test data source that will be implemented by descendants");

	public static void SerializeUndersizedBufferTestData() =>
		throw new NotImplementedException("This is just a placeholder for the test data source that will be implemented by descendants");

	public static void DeserializeUndersizedBufferTestData() =>
		throw new NotImplementedException("This is just a placeholder for the test data source that will be implemented by descendants");

	public static void ByteCountTestData() =>
		throw new NotImplementedException("This is just a placeholder for the test data source that will be implemented by descendants");
}
