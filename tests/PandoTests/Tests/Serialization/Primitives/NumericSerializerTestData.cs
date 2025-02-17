using Pando.Serialization.Primitives;
using Xunit;

namespace PandoTests.Tests.Serialization.Primitives;

public class SByteSerializerTestData
{
	public static TheoryData<sbyte, byte[], SByteSerializer> SerializationTestData => new()
	{
		{ sbyte.MaxValue, [0x7F], new SByteSerializer() },
	};
}

public class ByteSerializerTestData
{
	public static TheoryData<byte, byte[], ByteSerializer> SerializationTestData => new()
	{
		{ byte.MaxValue, [0xFF], new ByteSerializer() },
	};
}

public class Int16LittleEndianSerializerTestData
{
	public static TheoryData<short, byte[], Int16LittleEndianSerializer> SerializationTestData => new()
	{
		{ -16321, [0x3F, 0xC0], new Int16LittleEndianSerializer() },
	};
}

public class UInt16LittleEndianSerializerTestData
{
	public static TheoryData<ushort, byte[], UInt16LittleEndianSerializer> SerializationTestData => new()
	{
		{ 49215, [0x3F, 0xC0], new UInt16LittleEndianSerializer() },
	};
}

public class Int32LittleEndianSerializerTestData
{
	public static TheoryData<int, byte[], Int32LittleEndianSerializer> SerializationTestData => new()
	{
		{ -2143297521, [0x0F, 0xE0, 0x3F, 0x80], new Int32LittleEndianSerializer() },
	};
}

public class UInt32LittleEndianSerializerTestData
{
	public static TheoryData<uint, byte[], UInt32LittleEndianSerializer> SerializationTestData => new()
	{
		{ 2151669775, [0x0F, 0xE0, 0x3F, 0x80], new UInt32LittleEndianSerializer() },
	};
}

public class Int64LittleEndianSerializerTestData
{
	public static TheoryData<long, byte[], Int64LittleEndianSerializer> SerializationTestData => new()
	{
		{ -9205392754131862016, [0x00, 0xFE, 0x03, 0xF8, 0x0F, 0xE0, 0x3F, 0x80], new Int64LittleEndianSerializer() },
	};
}

public class UInt64LittleEndianSerializerTestData
{
	public static TheoryData<ulong, byte[], UInt64LittleEndianSerializer> SerializationTestData => new()
	{
		{ 9241351319577689600, [0x00, 0xFE, 0x03, 0xF8, 0x0F, 0xE0, 0x3F, 0x80], new UInt64LittleEndianSerializer() },
	};
}
