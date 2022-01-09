using Pando.Serialization.PrimitiveSerializers;
using Xunit;

// Rider doesn't detect subclasses of BaseSerializerTest as being used, because they inherit their test methods
// ReSharper disable UnusedType.Global

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public partial class PrimitiveSerializerTests
{
	public class SByteSerializerTest : BaseSerializerTest<sbyte>, ISerializerTestData<sbyte>
	{
		protected override IPrimitiveSerializer<sbyte> Serializer => new SByteSerializer();

		public static TheoryData<sbyte, byte[]> SerializationTestData => new()
		{
			{ sbyte.MaxValue, new byte[] { 0x7F } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { sizeof(byte) };
	}

	public class ByteSerializerTest : BaseSerializerTest<byte>, ISerializerTestData<byte>
	{
		protected override IPrimitiveSerializer<byte> Serializer => new ByteSerializer();

		public static TheoryData<byte, byte[]> SerializationTestData => new()
		{
			{ byte.MaxValue, new byte[] { 0xFF } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { sizeof(byte) };
	}

	public class Int16LittleEndianSerializerTest : BaseSerializerTest<short>, ISerializerTestData<short>
	{
		protected override IPrimitiveSerializer<short> Serializer => new Int16LittleEndianSerializer();

		public static TheoryData<short, byte[]> SerializationTestData => new()
		{
			{ -16321, new byte[] { 0x3F, 0xC0 } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { sizeof(short) };
	}

	public class UInt16LittleEndianSerializerTest : BaseSerializerTest<ushort>, ISerializerTestData<ushort>
	{
		protected override IPrimitiveSerializer<ushort> Serializer => new UInt16LittleEndianSerializer();

		public static TheoryData<ushort, byte[]> SerializationTestData => new()
		{
			{ 49215, new byte[] { 0x3F, 0xC0 } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { sizeof(ushort) };
	}

	public class Int32LittleEndianSerializerTest : BaseSerializerTest<int>, ISerializerTestData<int>
	{
		protected override IPrimitiveSerializer<int> Serializer => new Int32LittleEndianSerializer();

		public static TheoryData<int, byte[]> SerializationTestData => new()
		{
			{ -2143297521, new byte[] { 0x0F, 0xE0, 0x3F, 0x80 } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { sizeof(int) };
	}

	public class UInt32LittleEndianSerializerTest : BaseSerializerTest<uint>, ISerializerTestData<uint>
	{
		protected override IPrimitiveSerializer<uint> Serializer => new UInt32LittleEndianSerializer();

		public static TheoryData<uint, byte[]> SerializationTestData => new()
		{
			{ 2151669775, new byte[] { 0x0F, 0xE0, 0x3F, 0x80 } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { sizeof(uint) };
	}

	public class Int64LittleEndianSerializerTest : BaseSerializerTest<long>, ISerializerTestData<long>
	{
		protected override IPrimitiveSerializer<long> Serializer => new Int64LittleEndianSerializer();

		public static TheoryData<long, byte[]> SerializationTestData => new()
		{
			{ -9205392754131862016, new byte[] { 0x00, 0xFE, 0x03, 0xF8, 0x0F, 0xE0, 0x3F, 0x80 } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { sizeof(long) };
	}

	public class UInt64LittleEndianSerializerTest : BaseSerializerTest<ulong>, ISerializerTestData<ulong>
	{
		protected override IPrimitiveSerializer<ulong> Serializer => new UInt64LittleEndianSerializer();

		public static TheoryData<ulong, byte[]> SerializationTestData => new()
		{
			{ 9241351319577689600, new byte[] { 0x00, 0xFE, 0x03, 0xF8, 0x0F, 0xE0, 0x3F, 0x80 } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { sizeof(ulong) };
	}
}
