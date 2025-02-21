using System;
using FluentAssertions;
using Pando.DataSources;
using Pando.Serialization;
using Pando.Serialization.Primitives;
using Xunit;

namespace PandoTests.Tests.Serialization.Primitives;

public class NullableSerializerTests
{
	private class InnerSerializer : IPandoSerializer<byte>
	{
		public int SerializedSize => 1;
		public void Serialize(byte value, Span<byte> buffer, INodeDataStore _) => buffer.Fill(byte.MaxValue);
		public byte Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore) => byte.MaxValue;
	}

	private static NullableSerializer<byte> Serializer() => new(new InnerSerializer());

	public static TheoryData<byte?, byte[], NullableSerializer<byte>> SerializationTestData => new()
	{
		{ null, [0, 0],       Serializer() },
		{ byte.MaxValue,    [0x01, 0xFF], Serializer() },
	};


	[Theory]
	[MemberData(nameof(SerializationTestData))]
	public void Serialize_should_produce_correct_bytes(byte? inputValue, byte[] expectedBytes, NullableSerializer<byte> serializer)
	{
		Span<byte> nodeBytes = stackalloc byte[expectedBytes.Length];
		var writeBuffer = nodeBytes;

		serializer.Serialize(inputValue, writeBuffer, null!);

		var serializationResult = nodeBytes[..expectedBytes.Length].ToArray();
		serializationResult.Should().BeEquivalentTo(expectedBytes);
	}

	[Theory]
	[MemberData(nameof(SerializationTestData))]
	public void Serialize_should_throw_when_buffer_is_too_small(byte? inputValue, byte[] expectedBytes, NullableSerializer<byte> serializer)
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

	[Theory]
	[MemberData(nameof(SerializationTestData))]
	public void Deserialize_should_produce_correct_value(byte? expectedValue, byte[] inputBytes, NullableSerializer<byte> serializer)
	{
		Span<byte> nodeBytes = new byte[inputBytes.Length];
		inputBytes.CopyTo(nodeBytes);
		ReadOnlySpan<byte> readBuffer = nodeBytes;

		var deserializationResult = serializer.Deserialize(readBuffer, null!);

		deserializationResult.Should().Be(expectedValue);
	}

#pragma warning disable xUnit1026 // ignore unused parameter warning; we want to use the same data for multiple tests

	[Theory]
	[MemberData(nameof(SerializationTestData))]
	public void Deserialize_should_throw_when_buffer_is_too_small(byte? _, byte[] inputBytes, NullableSerializer<byte> serializer)
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
