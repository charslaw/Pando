using Pando.Serialization.PrimitiveSerializers;
using Xunit;

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public class SByteSerializerTestData
{
	private static SByteSerializer Serializer() => new();

	public static TheoryData<sbyte, byte[], SByteSerializer> SerializationTestData => new()
	{
		{ sbyte.MaxValue, [0x7F], Serializer() },
	};

	public static TheoryData<sbyte, int?, SByteSerializer> ByteCountTestData => new()
	{
		{ 0, sizeof(sbyte), Serializer() },
	};
}

public class ByteSerializerTestData
{
	private static ByteSerializer Serializer() => new();

	public static TheoryData<byte, byte[], ByteSerializer> SerializationTestData => new()
	{
		{ byte.MaxValue, [0xFF], Serializer() },
	};

	public static TheoryData<byte, int?, ByteSerializer> ByteCountTestData => new()
	{
		{ 0, sizeof(byte), Serializer() },
	};
}

public class Int16LittleEndianSerializerTestData
{
	private static Int16LittleEndianSerializer Serializer() => new();

	public static TheoryData<short, byte[], Int16LittleEndianSerializer> SerializationTestData => new()
	{
		{ -16321, [0x3F, 0xC0], Serializer() },
	};

	public static TheoryData<short, int?, Int16LittleEndianSerializer> ByteCountTestData => new()
	{
		{ 0, sizeof(short), Serializer() },
	};
}

public class UInt16LittleEndianSerializerTestData
{
	private static UInt16LittleEndianSerializer Serializer() => new();

	public static TheoryData<ushort, byte[], UInt16LittleEndianSerializer> SerializationTestData => new()
	{
		{ 49215, [0x3F, 0xC0], Serializer() },
	};

	public static TheoryData<ushort, int?, UInt16LittleEndianSerializer> ByteCountTestData => new()
	{
		{ 0, sizeof(ushort), Serializer() },
	};
}

public class Int32LittleEndianSerializerTestData
{
	private static Int32LittleEndianSerializer Serializer() => new();

	public static TheoryData<int, byte[], Int32LittleEndianSerializer> SerializationTestData => new()
	{
		{ -2143297521, [0x0F, 0xE0, 0x3F, 0x80], Serializer() },
	};

	public static TheoryData<int, int?, Int32LittleEndianSerializer> ByteCountTestData => new()
	{
		{ 0, sizeof(int), Serializer() },
	};
}

public class UInt32LittleEndianSerializerTestData
{
	private static UInt32LittleEndianSerializer Serializer() => new();

	public static TheoryData<uint, byte[], UInt32LittleEndianSerializer> SerializationTestData => new()
	{
		{ 2151669775, [0x0F, 0xE0, 0x3F, 0x80], Serializer() },
	};

	public static TheoryData<uint, int?, UInt32LittleEndianSerializer> ByteCountTestData => new()
	{
		{ 0, sizeof(uint), Serializer() },
	};
}

public class Int64LittleEndianSerializerTestData
{
	private static Int64LittleEndianSerializer Serializer() => new();

	public static TheoryData<long, byte[], Int64LittleEndianSerializer> SerializationTestData => new()
	{
		{ -9205392754131862016, [0x00, 0xFE, 0x03, 0xF8, 0x0F, 0xE0, 0x3F, 0x80], Serializer() },
	};

	public static TheoryData<long, int?, Int64LittleEndianSerializer> ByteCountTestData => new()
	{
		{ 0, sizeof(long), Serializer() },
	};
}

public class UInt64LittleEndianSerializerTestData
{
	private static UInt64LittleEndianSerializer Serializer() => new();

	public static TheoryData<ulong, byte[], UInt64LittleEndianSerializer> SerializationTestData => new()
	{
		{ 9241351319577689600, [0x00, 0xFE, 0x03, 0xF8, 0x0F, 0xE0, 0x3F, 0x80], Serializer() },
	};

	public static TheoryData<ulong, int?, UInt64LittleEndianSerializer> ByteCountTestData => new()
	{
		{ 0, sizeof(ulong), Serializer() },
	};
}
