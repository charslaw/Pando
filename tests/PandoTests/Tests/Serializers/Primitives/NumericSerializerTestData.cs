using System;
using System.Collections.Generic;
using Pando.Serializers;
using Pando.Serializers.Primitives;

namespace PandoTests.Tests.Serializers.Primitives;

[InheritsTests]
public class SByteSerializerTests : PrimitiveSerializerTest<sbyte>
{
	public override IPandoSerializer<sbyte> CreateSerializer() => new SByteSerializer();

	public override IEnumerable<Func<(sbyte, byte[])>> SerializationTestData()
	{
		yield return () => (sbyte.MaxValue, [0x7F]);
	}
}

[InheritsTests]
public class ByteSerializerTests : PrimitiveSerializerTest<byte>
{
	public override IPandoSerializer<byte> CreateSerializer() => new ByteSerializer();

	public override IEnumerable<Func<(byte, byte[])>> SerializationTestData()
	{
		yield return () => (byte.MaxValue, [0xFF]);
	}
}

[InheritsTests]
public class Int16LittleEndianSerializerTests : PrimitiveSerializerTest<short>
{
	public override IPandoSerializer<short> CreateSerializer() => new Int16LittleEndianSerializer();

	public override IEnumerable<Func<(short, byte[])>> SerializationTestData()
	{
		yield return () => (-16321, [0x3F, 0xC0]);
	}
}

[InheritsTests]
public class UInt16LittleEndianSerializerTests : PrimitiveSerializerTest<ushort>
{
	public override IPandoSerializer<ushort> CreateSerializer() => new UInt16LittleEndianSerializer();

	public override IEnumerable<Func<(ushort, byte[])>> SerializationTestData()
	{
		yield return () => (49215, [0x3F, 0xC0]);
	}
}

[InheritsTests]
public class Int32LittleEndianSerializerTests : PrimitiveSerializerTest<int>
{
	public override IPandoSerializer<int> CreateSerializer() => new Int32LittleEndianSerializer();

	public override IEnumerable<Func<(int, byte[])>> SerializationTestData()
	{
		yield return () => (-2143297521, [0x0F, 0xE0, 0x3F, 0x80]);
	}
}

[InheritsTests]
public class UInt32LittleEndianSerializerTests : PrimitiveSerializerTest<uint>
{
	public override IPandoSerializer<uint> CreateSerializer() => new UInt32LittleEndianSerializer();

	public override IEnumerable<Func<(uint, byte[])>> SerializationTestData()
	{
		yield return () => (2151669775, [0x0F, 0xE0, 0x3F, 0x80]);
	}
}

[InheritsTests]
public class Int64LittleEndianSerializerTests : PrimitiveSerializerTest<long>
{
	public override IPandoSerializer<long> CreateSerializer() => new Int64LittleEndianSerializer();

	public override IEnumerable<Func<(long, byte[])>> SerializationTestData()
	{
		yield return () => (-9205392754131862016, [0x00, 0xFE, 0x03, 0xF8, 0x0F, 0xE0, 0x3F, 0x80]);
	}
}

[InheritsTests]
public class UInt64LittleEndianSerializerTests : PrimitiveSerializerTest<ulong>
{
	public override IPandoSerializer<ulong> CreateSerializer() => new UInt64LittleEndianSerializer();

	public override IEnumerable<Func<(ulong, byte[])>> SerializationTestData()
	{
		yield return () => (9241351319577689600, [0x00, 0xFE, 0x03, 0xF8, 0x0F, 0xE0, 0x3F, 0x80]);
	}
}
