using System;
using System.Buffers.Binary;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

// Rider doesn't detect subclasses of BaseSerializerTest as being used, because they inherit their test methods
// ReSharper disable UnusedType.Global

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public class EnumSerializerTests : BaseSerializerTest<TestEnum>, ISerializerTestData<TestEnum>
{
	protected override IPrimitiveSerializer<TestEnum> Serializer => new EnumSerializer<TestEnum, long>(new SimpleLongSerializer());

	public static TheoryData<TestEnum, byte[]> SerializationTestData => new()
	{
		{ TestEnum.Value, new byte[] { 0xFF, 0x7F, 0x3F, 0x1F, 0x0F, 0x07, 0x03, 0x01 } },
	};

	public static TheoryData<int?> ByteCountTestData => new() { sizeof(ulong) };
}

public enum TestEnum : long
{
	Value = -36_240_869_367_020_799 // 0xFF_7F_3F_1F_0F_07_03_01
}

internal class SimpleLongSerializer : IPrimitiveSerializer<long>
{
	public int? ByteCount => sizeof(long);
	public int ByteCountForValue(long value) => sizeof(long);

	public void Serialize(long value, ref Span<byte> buffer)
	{
		BinaryPrimitives.WriteInt64BigEndian(buffer, value);
		buffer = buffer[sizeof(ulong)..];
	}

	public long Deserialize(ref ReadOnlySpan<byte> buffer)
	{
		var result = BinaryPrimitives.ReadInt64BigEndian(buffer);
		buffer = buffer[sizeof(ulong)..];
		return result;
	}
}
