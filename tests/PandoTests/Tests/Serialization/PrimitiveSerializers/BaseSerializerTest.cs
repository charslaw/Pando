using System;
using FluentAssertions;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public abstract class BaseSerializerTest<T>
{
	protected abstract IPrimitiveSerializer<T> Serializer { get; }

	[Theory]
	[MemberData(nameof(TestDataProviderPlaceholders.SerializationTestData))]
	public virtual void Serialize_should_produce_correct_bytes(T value, byte[] bytes)
	{
		Span<byte> nodeBytes = stackalloc byte[bytes.Length];
		var writeBuffer = nodeBytes;
		Serializer.Serialize(value, ref writeBuffer);
		var serializationResult = nodeBytes.ToArray();

		serializationResult.Should().BeEquivalentTo(bytes);
	}

	[Theory]
	[MemberData(nameof(TestDataProviderPlaceholders.SerializationTestData))]
	public virtual void Deserialize_should_produce_correct_value(T value, byte[] bytes)
	{
		var bytesSpan = new ReadOnlySpan<byte>(bytes);
		var deserializationResult = Serializer.Deserialize(ref bytesSpan);
		deserializationResult.Should().BeEquivalentTo(value);
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
