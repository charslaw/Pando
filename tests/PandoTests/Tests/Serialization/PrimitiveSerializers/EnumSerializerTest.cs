using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public record struct TestData<T>(T Value, byte[] Bytes);

public class EnumSerializerTest
{
	[Theory]
	[ClassData(typeof(SByteEnumSerializerTestData))]
	[ClassData(typeof(ByteEnumSerializerTestData))]
	[ClassData(typeof(Int16EnumSerializerTestData))]
	[ClassData(typeof(UInt16EnumSerializerTestData))]
	[ClassData(typeof(Int32EnumSerializerTestData))]
	[ClassData(typeof(UInt32EnumSerializerTestData))]
	[ClassData(typeof(Int64EnumSerializerTestData))]
	[ClassData(typeof(UInt64EnumSerializerTestData))]
	public void Serialize_Deserialize<T>(TestData<T> testData, IPrimitiveSerializer<T> serializer)
	{
		var (value, bytes) = testData;

		// Serialize
		Span<byte> nodeBytes = stackalloc byte[bytes.Length];
		var writeBuffer = nodeBytes;
		serializer.Serialize(value, ref writeBuffer);
		var serializationResult = nodeBytes.ToArray();

		serializationResult.Should().BeEquivalentTo(bytes);

		// Deserialize
		var bytesSpan = new ReadOnlySpan<byte>(bytes);
		var deserializationResult = serializer.Deserialize(ref bytesSpan);

		deserializationResult.Should().BeEquivalentTo(value);
	}
}

public abstract class BaseTestData<T> : IEnumerable<object[]>
{
	protected abstract IPrimitiveSerializer<T> ProduceSerializer();
	protected abstract IEnumerable<TestData<T>> ProduceTestData();

	public IEnumerator<object[]> GetEnumerator() => ProduceTestData().Select(td => new object[] { td, ProduceSerializer() }).GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public abstract class BaseEnumTestData<T> : BaseTestData<T>
	where T : unmanaged, Enum
{
	protected override IPrimitiveSerializer<T> ProduceSerializer() => EnumSerializer.SerializerFor<T>();
}

public class SByteEnumSerializerTestData : BaseEnumTestData<SByteEnum>
{
	protected override IEnumerable<TestData<SByteEnum>> ProduceTestData() => new List<TestData<SByteEnum>>
	{
		new(SByteEnum.Min, new byte[] { 0x80 }),
		new(SByteEnum.Mid1, new byte[] { 0xC8 }),
		new(SByteEnum.Mid2, new byte[] { 0x5B }),
		new(SByteEnum.Max, new byte[] { 0x7F }),
	};
}

public class ByteEnumSerializerTestData : BaseEnumTestData<ByteEnum>
{
	protected override IEnumerable<TestData<ByteEnum>> ProduceTestData() => new List<TestData<ByteEnum>>
	{
		new(ByteEnum.Min, new byte[] { 0x00 }),
		new(ByteEnum.Mid1, new byte[] { 0x25 }),
		new(ByteEnum.Mid2, new byte[] { 0xB8 }),
		new(ByteEnum.Max, new byte[] { 0xFF }),
	};
}

public class Int16EnumSerializerTestData : BaseEnumTestData<Int16Enum>
{
	protected override IEnumerable<TestData<Int16Enum>> ProduceTestData() => new List<TestData<Int16Enum>>
	{
		new(Int16Enum.Min, new byte[] { 0x00, 0x80 }),
		new(Int16Enum.Mid1, new byte[] { 0x1F, 0xC8 }),
		new(Int16Enum.Mid2, new byte[] { 0xC0, 0x5B }),
		new(Int16Enum.Max, new byte[] { 0xFF, 0x7F }),
	};
}

public class UInt16EnumSerializerTestData : BaseEnumTestData<UInt16Enum>
{
	protected override IEnumerable<TestData<UInt16Enum>> ProduceTestData() => new List<TestData<UInt16Enum>>
	{
		new(UInt16Enum.Min, new byte[] { 0x00, 0x00 }),
		new(UInt16Enum.Mid1, new byte[] { 0x40, 0x24 }),
		new(UInt16Enum.Mid2, new byte[] { 0xE1, 0xB7 }),
		new(UInt16Enum.Max, new byte[] { 0xFF, 0xFF }),
	};
}

public class Int32EnumSerializerTestData : BaseEnumTestData<Int32Enum>
{
	protected override IEnumerable<TestData<Int32Enum>> ProduceTestData() => new List<TestData<Int32Enum>>
	{
		new(Int32Enum.Min, new byte[] { 0x00, 0x00, 0x00, 0x80 }),
		new(Int32Enum.Mid1, new byte[] { 0x9E, 0xAE, 0x1E, 0xC8 }),
		new(Int32Enum.Mid2, new byte[] { 0x77, 0x95, 0xC0, 0x5B }),
		new(Int32Enum.Max, new byte[] { 0xFF, 0xFF, 0xFF, 0x7F }),
	};
}

public class UInt32EnumSerializerTestData : BaseEnumTestData<UInt32Enum>
{
	protected override IEnumerable<TestData<UInt32Enum>> ProduceTestData() => new List<TestData<UInt32Enum>>
	{
		new(UInt32Enum.Min, new byte[] { 0x00, 0x00, 0x00, 0x00 }),
		new(UInt32Enum.Mid1, new byte[] { 0x89, 0x6A, 0x3F, 0x24 }),
		new(UInt32Enum.Mid2, new byte[] { 0x62, 0x51, 0xE1, 0xB7 }),
		new(UInt32Enum.Max, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }),
	};
}

public class Int64EnumSerializerTestData : BaseEnumTestData<Int64Enum>
{
	protected override IEnumerable<TestData<Int64Enum>> ProduceTestData() => new List<TestData<Int64Enum>>
	{
		new(Int64Enum.Min, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80 }),
		new(Int64Enum.Mid1, new byte[] { 0x00, 0xDC, 0x12, 0x75, 0x9D, 0xAE, 0x1E, 0xC8 }),
		new(Int64Enum.Mid2, new byte[] { 0x00, 0xF8, 0x5C, 0x7A, 0x77, 0x95, 0xC0, 0x5B }),
		new(Int64Enum.Max, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F }),
	};
}

public class UInt64EnumSerializerTestData : BaseEnumTestData<UInt64Enum>
{
	protected override IEnumerable<TestData<UInt64Enum>> ProduceTestData() => new List<TestData<UInt64Enum>>
	{
		new(UInt64Enum.Min, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
		new(UInt64Enum.Mid1, new byte[] { 0x00, 0xFE, 0xA2, 0x85, 0x88, 0x6A, 0x3F, 0x24 }),
		new(UInt64Enum.Mid2, new byte[] { 0x00, 0x18, 0xED, 0x8A, 0x62, 0x51, 0xE1, 0xB7 }),
		new(UInt64Enum.Max, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }),
	};
}
