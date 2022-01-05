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

	public static void ByteCountTestData() =>
		throw new NotImplementedException("This is just a placeholder for the test data source that will be implemented by descendants");
}
