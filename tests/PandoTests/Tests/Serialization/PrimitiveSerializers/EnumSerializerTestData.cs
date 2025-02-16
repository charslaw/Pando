using Pando.Serialization.PrimitiveSerializers;
using Xunit;

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public class EnumSerializerTestData
{
	private static EnumSerializer<TestEnum, long> Serializer() => new(new SimpleLongSerializer());
	
	public static TheoryData<TestEnum, byte[], EnumSerializer<TestEnum, long>> SerializationTestData => new()
	{
		{ TestEnum.Value, [0xFF, 0x7F, 0x3F, 0x1F, 0x0F, 0x07, 0x03, 0x01], Serializer() },
	};

	public static TheoryData<TestEnum, int?, EnumSerializer<TestEnum, long>> ByteCountTestData => new()
	{
		{ default, sizeof(ulong), Serializer() }
	};
}

public enum TestEnum : long
{
	Value = -36_240_869_367_020_799 // 0xFF_7F_3F_1F_0F_07_03_01
}
