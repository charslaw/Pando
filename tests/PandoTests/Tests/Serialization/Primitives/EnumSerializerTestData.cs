using Pando.Serialization.Primitives;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

namespace PandoTests.Tests.Serialization.Primitives;

public class EnumSerializerTestData
{
	public static TheoryData<TestEnum, byte[], EnumSerializer<TestEnum, long>> SerializationTestData => new()
	{
		{
			TestEnum.Value,
			[0x01, 0x03, 0x07, 0x0F, 0x1F, 0x3F, 0x7F, 0xFF], // little endian
			new EnumSerializer<TestEnum, long>(Int64LittleEndianSerializer.Default)
		},
	};
}

public enum TestEnum : long
{
	Value = -36_240_869_367_020_799 // 0xFF_7F_3F_1F_0F_07_03_01
}
