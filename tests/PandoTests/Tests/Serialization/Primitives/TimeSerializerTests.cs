using System;
using System.Collections.Generic;
using Pando.Serialization;
using Pando.Serialization.Primitives;

namespace PandoTests.Tests.Serialization.Primitives;

[InheritsTests]
public class DateTimeToBinarySerializerTests : PrimitiveSerializerTest<DateTime>
{
	public override IPandoSerializer<DateTime> CreateSerializer() =>
		new DateTimeToBinarySerializer(new Int64LittleEndianSerializer());

	public override IEnumerable<Func<(DateTime, byte[])>> SerializationTestData()
	{
		yield return () =>
			(
				new DateTime(1415, 10, 25, 12, 34, 56, 789, DateTimeKind.Utc),
				[0x50, 0x3C, 0xBC, 0x6E, 0x8B, 0x2F, 0x32, 0x46] // Little endian
			);
	}

	[Test]
	[MethodDataSource(nameof(SerializationTestData))]
	public override async Task Deserialize_should_produce_correct_value(DateTime expectedValue, byte[] inputBytes)
	{
		// Over-allocate read buffer to ensure that the serializer doesn't get greedy with reading from the read buffer
		// If the serializer reads into the extra buffer space, presumably it will not produce the correct result.
		Span<byte> nodeBytes = new byte[inputBytes.Length + EXTRA_BUFFER_SPACE];
		inputBytes.CopyTo(nodeBytes);

		var deserializationResult = CreateSerializer().Deserialize(nodeBytes, null!);

		using (Assert.Multiple())
		{
			await Assert.That(deserializationResult).IsEqualTo(expectedValue);
			await Assert.That(deserializationResult.Kind).IsEqualTo(expectedValue.Kind);
		}
	}
}

[InheritsTests]
public class TimeSpanTicksSerializerTests : PrimitiveSerializerTest<TimeSpan>
{
	public override IPandoSerializer<TimeSpan> CreateSerializer() =>
		new TimeSpanTicksSerializer(new Int64LittleEndianSerializer());

	public override IEnumerable<Func<(TimeSpan, byte[])>> SerializationTestData()
	{
		yield return () =>
			(
				new TimeSpan(1234567, 89, 87, 65, 4321),
				[0x90, 0xBD, 0x3A, 0x92, 0xEF, 0x91, 0xCD, 0x0E] // little endian
			);
	}
}

[InheritsTests]
public class DateOnlyDayNumberSerializerTests : PrimitiveSerializerTest<DateOnly>
{
	public override IPandoSerializer<DateOnly> CreateSerializer() =>
		new DateOnlyDayNumberSerializer(new Int32LittleEndianSerializer());

	public override IEnumerable<Func<(DateOnly, byte[])>> SerializationTestData()
	{
		yield return () =>
			(
				DateOnly.MaxValue,
				[0xDA, 0xB9, 0x37, 0x00] // little endian
			);
	}
}

[InheritsTests]
public class TimeOnlyDayNumberSerializerTests : PrimitiveSerializerTest<TimeOnly>
{
	public override IPandoSerializer<TimeOnly> CreateSerializer() =>
		new TimeOnlyTicksSerializer(new Int64LittleEndianSerializer());

	public override IEnumerable<Func<(TimeOnly, byte[])>> SerializationTestData()
	{
		yield return () =>
			(
				TimeOnly.MaxValue,
				[0xFF, 0xBF, 0x69, 0x2A, 0xC9, 0x00, 0x00, 0x00] // little endian
			);
	}
}
