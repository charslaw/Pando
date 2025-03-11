using System;
using System.Collections.Generic;
using Pando.Serializers;
using Pando.Serializers.Primitives;

namespace PandoTests.Tests.Serializers.Primitives;

[InheritsTests]
public class EnumSerializerTests : PrimitiveSerializerTest<TestEnum>
{
	public override IPandoSerializer<TestEnum> CreateSerializer() =>
		new EnumSerializer<TestEnum, long>(Int64LittleEndianSerializer.Default);

	public override IEnumerable<Func<(TestEnum, byte[])>> SerializationTestData()
	{
		yield return () =>
			(
				TestEnum.Value,
				[0x01, 0x03, 0x07, 0x0F, 0x1F, 0x3F, 0x7F, 0xFF] // little endian
			);
	}
}

public enum TestEnum : long
{
	Value = -36_240_869_367_020_799 // 0xFF_7F_3F_1F_0F_07_03_01
}
