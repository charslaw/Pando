using System;
using FluentAssertions;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public abstract class BaseSerializerTest<T>
{
	protected abstract IPrimitiveSerializer<T> Serializer { get; }

	private TheoryData<T, byte[]> TestData() =>
		throw new NotImplementedException("This is just a placeholder for the test data source that will be implemented by descendants");

	[Theory]
	[MemberData(nameof(TestData))]
	public virtual void Should_produce_correct_bytes(T value, byte[] bytes)
	{
		Span<byte> nodeBytes = stackalloc byte[bytes.Length];
		var writeBuffer = nodeBytes;
		Serializer.Serialize(value, ref writeBuffer);
		var serializationResult = nodeBytes.ToArray();

		serializationResult.Should().BeEquivalentTo(bytes);
	}

	[Theory]
	[MemberData(nameof(TestData))]
	public virtual void Should_produce_correct_value(T value, byte[] bytes)
	{
		var bytesSpan = new ReadOnlySpan<byte>(bytes);
		var deserializationResult = Serializer.Deserialize(ref bytesSpan);
		deserializationResult.Should().BeEquivalentTo(value);
	}
}
