using System;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

// Rider doesn't detect subclasses of BaseSerializerTest as being used, because they inherit their test methods
// ReSharper disable UnusedType.Global

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public class DateTimeToBinarySerializerTestData
{
	private static DateTimeToBinarySerializer Serializer() => new(new SimpleLongSerializer());

	public static TheoryData<DateTime, byte[], DateTimeToBinarySerializer> SerializationTestData => new()
	{
		{ new DateTime(1415, 10, 25, 12, 34, 56, 789, DateTimeKind.Utc), [0x46, 0x32, 0x2F, 0x8B, 0x6E, 0xBC, 0x3C, 0x50], Serializer() }
	};

	public static TheoryData<DateTime, int?, DateTimeToBinarySerializer> ByteCountTestData => new()
	{
		{ default, sizeof(long), Serializer() },
	};
}

public class TimeSpanTicksSerializerTestData
{
	private static TimeSpanTicksSerializer Serializer() => new(new SimpleLongSerializer());

	public static TheoryData<TimeSpan, byte[], TimeSpanTicksSerializer> SerializationTestData => new()
	{
		{ new TimeSpan(1234567, 89, 87, 65, 4321), [0x0E, 0xCD, 0x91, 0xEF, 0x92, 0x3A, 0xBD, 0x90], Serializer() },
	};

	public static TheoryData<TimeSpan, int?, TimeSpanTicksSerializer> ByteCountTestData => new()
	{
		{ TimeSpan.Zero, sizeof(long), Serializer() },
	};
}

public class DateOnlyDayNumberSerializerTestData
{
	private static DateOnlyDayNumberSerializer Serializer() => new(new SimpleIntSerializer());

	public static TheoryData<DateOnly, byte[], DateOnlyDayNumberSerializer> SerializationTestData => new()
	{
		{ DateOnly.MaxValue, [0x00, 0x37, 0xB9, 0xDA], Serializer() }
	};

	public static TheoryData<DateOnly, int?, DateOnlyDayNumberSerializer> ByteCountTestData => new()
	{
		{ default, sizeof(int), Serializer() },
	};
}

public class TimeOnlyDayNumberSerializerTestData
{
	private static TimeOnlyTicksSerializer Serializer() => new(new SimpleLongSerializer());

	public static TheoryData<TimeOnly, byte[], TimeOnlyTicksSerializer> SerializationTestData => new()
	{
		{ TimeOnly.MaxValue, [0x00, 0x00, 0x00, 0xC9, 0x2A, 0x69, 0xBF, 0xFF], Serializer() }
	};

	public static TheoryData<TimeOnly, int?, TimeOnlyTicksSerializer> ByteCountTestData => new()
	{
		{ default, sizeof(long), Serializer() },
	};
}
