using System;
using Pando.Serialization.Primitives;
using Xunit;

namespace PandoTests.Tests.Serialization.Primitives;

public class DateTimeToBinarySerializerTestData
{
	public static TheoryData<DateTime, byte[], DateTimeToBinarySerializer> SerializationTestData => new()
	{
		{
			new DateTime(1415, 10, 25, 12, 34, 56, 789, DateTimeKind.Utc),
			[0x50, 0x3C, 0xBC, 0x6E, 0x8B, 0x2F, 0x32, 0x46], // Little endian
			new DateTimeToBinarySerializer(new Int64LittleEndianSerializer())
		}
	};
}

public class TimeSpanTicksSerializerTestData
{
	public static TheoryData<TimeSpan, byte[], TimeSpanTicksSerializer> SerializationTestData => new()
	{
		{
			new TimeSpan(1234567, 89, 87, 65, 4321),
			[0x90, 0xBD, 0x3A, 0x92, 0xEF, 0x91, 0xCD, 0x0E], // little endian
			new TimeSpanTicksSerializer(new Int64LittleEndianSerializer())
		},
	};
}

public class DateOnlyDayNumberSerializerTestData
{
	public static TheoryData<DateOnly, byte[], DateOnlyDayNumberSerializer> SerializationTestData => new()
	{
		{
			DateOnly.MaxValue,
			[0xDA, 0xB9, 0x37, 0x00], // little endian
			new DateOnlyDayNumberSerializer(new Int32LittleEndianSerializer())
		}
	};
}

public class TimeOnlyDayNumberSerializerTestData
{

	public static TheoryData<TimeOnly, byte[], TimeOnlyTicksSerializer> SerializationTestData => new()
	{
		{
			TimeOnly.MaxValue,
			[0xFF, 0xBF, 0x69, 0x2A, 0xC9, 0x00, 0x00, 0x00], // little endian
			new TimeOnlyTicksSerializer(new Int64LittleEndianSerializer())
		}
	};
}
