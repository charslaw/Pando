using System;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

// Rider doesn't detect subclasses of BaseSerializerTest as being used, because they inherit their test methods
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

/// A collection of test classes for every kind of enum serializer
/// <see cref="BaseSerializerTest{T}"/>
public class EnumSerializerTests
{
	public class SByteEnumSerializerTests : BaseSerializerTest<SByteEnum>
	{
		private const int EXPECTED_SIZE = sizeof(sbyte);

		protected override IPrimitiveSerializer<SByteEnum> Serializer => EnumSerializer.SerializerFor<SByteEnum>();

		public static TheoryData<SByteEnum, byte[]> SerializationTestData => new()
		{
			{ SByteEnum.Min, new byte[] { 0x80 } },
			{ SByteEnum.Mid1, new byte[] { 0xC8 } },
			{ SByteEnum.Mid2, new byte[] { 0x5B } },
			{ SByteEnum.Max, new byte[] { 0x7F } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { EXPECTED_SIZE };
		public static TheoryData<SByteEnum, int> SerializeUndersizedBufferTestData => new() { { SByteEnum.Min, EXPECTED_SIZE } };
		public static TheoryData<byte[]> DeserializeUndersizedBufferTestData => new() { Array.Empty<byte>() };
	}

	public class ByteEnumSerializerTests : BaseSerializerTest<ByteEnum>
	{
		private const int EXPECTED_SIZE = sizeof(byte);

		protected override IPrimitiveSerializer<ByteEnum> Serializer => EnumSerializer.SerializerFor<ByteEnum>();

		public static TheoryData<ByteEnum, byte[]> SerializationTestData => new()
		{
			{ ByteEnum.Min, new byte[] { 0x00 } },
			{ ByteEnum.Mid1, new byte[] { 0x25 } },
			{ ByteEnum.Mid2, new byte[] { 0xB8 } },
			{ ByteEnum.Max, new byte[] { 0xFF } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { EXPECTED_SIZE };
		public static TheoryData<ByteEnum, int> SerializeUndersizedBufferTestData => new() { { ByteEnum.Min, EXPECTED_SIZE } };
		public static TheoryData<byte[]> DeserializeUndersizedBufferTestData => new() { Array.Empty<byte>() };
	}

	public class Int16EnumSerializerTests : BaseSerializerTest<Int16Enum>
	{
		private const int EXPECTED_SIZE = sizeof(short);

		protected override IPrimitiveSerializer<Int16Enum> Serializer => EnumSerializer.SerializerFor<Int16Enum>();

		public static TheoryData<Int16Enum, byte[]> SerializationTestData => new()
		{
			{ Int16Enum.Min, new byte[] { 0x00, 0x80 } },
			{ Int16Enum.Mid1, new byte[] { 0x1F, 0xC8 } },
			{ Int16Enum.Mid2, new byte[] { 0xC0, 0x5B } },
			{ Int16Enum.Max, new byte[] { 0xFF, 0x7F } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { EXPECTED_SIZE };
		public static TheoryData<Int16Enum, int> SerializeUndersizedBufferTestData => new() { { Int16Enum.Min, EXPECTED_SIZE } };
		public static TheoryData<byte[]> DeserializeUndersizedBufferTestData => new() { Array.Empty<byte>() };
	}

	public class UInt16EnumSerializerTests : BaseSerializerTest<UInt16Enum>
	{
		private const int EXPECTED_SIZE = sizeof(ushort);

		protected override IPrimitiveSerializer<UInt16Enum> Serializer => EnumSerializer.SerializerFor<UInt16Enum>();

		public static TheoryData<UInt16Enum, byte[]> SerializationTestData => new()
		{
			{ UInt16Enum.Min, new byte[] { 0x00, 0x00 } },
			{ UInt16Enum.Mid1, new byte[] { 0x40, 0x24 } },
			{ UInt16Enum.Mid2, new byte[] { 0xE1, 0xB7 } },
			{ UInt16Enum.Max, new byte[] { 0xFF, 0xFF } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { EXPECTED_SIZE };
		public static TheoryData<UInt16Enum, int> SerializeUndersizedBufferTestData => new() { { UInt16Enum.Min, EXPECTED_SIZE } };
		public static TheoryData<byte[]> DeserializeUndersizedBufferTestData => new() { Array.Empty<byte>() };
	}

	public class Int32EnumSerializerTests : BaseSerializerTest<Int32Enum>
	{
		private const int EXPECTED_SIZE = sizeof(int);

		protected override IPrimitiveSerializer<Int32Enum> Serializer => EnumSerializer.SerializerFor<Int32Enum>();

		public static TheoryData<Int32Enum, byte[]> SerializationTestData => new()
		{
			{ Int32Enum.Min, new byte[] { 0x00, 0x00, 0x00, 0x80 } },
			{ Int32Enum.Mid1, new byte[] { 0x9E, 0xAE, 0x1E, 0xC8 } },
			{ Int32Enum.Mid2, new byte[] { 0x77, 0x95, 0xC0, 0x5B } },
			{ Int32Enum.Max, new byte[] { 0xFF, 0xFF, 0xFF, 0x7F } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { EXPECTED_SIZE };
		public static TheoryData<Int32Enum, int> SerializeUndersizedBufferTestData => new() { { Int32Enum.Min, EXPECTED_SIZE } };
		public static TheoryData<byte[]> DeserializeUndersizedBufferTestData => new() { Array.Empty<byte>() };
	}

	public class UInt32EnumSerializerTests : BaseSerializerTest<UInt32Enum>
	{
		private const int EXPECTED_SIZE = sizeof(uint);

		protected override IPrimitiveSerializer<UInt32Enum> Serializer => EnumSerializer.SerializerFor<UInt32Enum>();

		public static TheoryData<UInt32Enum, byte[]> SerializationTestData => new()
		{
			{ UInt32Enum.Min, new byte[] { 0x00, 0x00, 0x00, 0x00 } },
			{ UInt32Enum.Mid1, new byte[] { 0x89, 0x6A, 0x3F, 0x24 } },
			{ UInt32Enum.Mid2, new byte[] { 0x62, 0x51, 0xE1, 0xB7 } },
			{ UInt32Enum.Max, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { EXPECTED_SIZE };
		public static TheoryData<UInt32Enum, int> SerializeUndersizedBufferTestData => new() { { UInt32Enum.Min, EXPECTED_SIZE } };
		public static TheoryData<byte[]> DeserializeUndersizedBufferTestData => new() { Array.Empty<byte>() };
	}

	public class Int64EnumSerializerTests : BaseSerializerTest<Int64Enum>
	{
		private const int EXPECTED_SIZE = sizeof(long);

		protected override IPrimitiveSerializer<Int64Enum> Serializer => EnumSerializer.SerializerFor<Int64Enum>();

		public static TheoryData<Int64Enum, byte[]> SerializationTestData => new()
		{
			{ Int64Enum.Min, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80 } },
			{ Int64Enum.Mid1, new byte[] { 0x00, 0xDC, 0x12, 0x75, 0x9D, 0xAE, 0x1E, 0xC8 } },
			{ Int64Enum.Mid2, new byte[] { 0x00, 0xF8, 0x5C, 0x7A, 0x77, 0x95, 0xC0, 0x5B } },
			{ Int64Enum.Max, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { EXPECTED_SIZE };
		public static TheoryData<Int64Enum, int> SerializeUndersizedBufferTestData => new() { { Int64Enum.Min, EXPECTED_SIZE } };
		public static TheoryData<byte[]> DeserializeUndersizedBufferTestData => new() { Array.Empty<byte>() };
	}

	public class UInt64EnumSerializerTests : BaseSerializerTest<UInt64Enum>
	{
		private const int EXPECTED_SIZE = sizeof(ulong);

		protected override IPrimitiveSerializer<UInt64Enum> Serializer => EnumSerializer.SerializerFor<UInt64Enum>();

		public static TheoryData<UInt64Enum, byte[]> SerializationTestData => new()
		{
			{ UInt64Enum.Min, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
			{ UInt64Enum.Mid1, new byte[] { 0x00, 0xFE, 0xA2, 0x85, 0x88, 0x6A, 0x3F, 0x24 } },
			{ UInt64Enum.Mid2, new byte[] { 0x00, 0x18, 0xED, 0x8A, 0x62, 0x51, 0xE1, 0xB7 } },
			{ UInt64Enum.Max, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { EXPECTED_SIZE };
		public static TheoryData<UInt64Enum, int> SerializeUndersizedBufferTestData => new() { { UInt64Enum.Min, EXPECTED_SIZE } };
		public static TheoryData<byte[]> DeserializeUndersizedBufferTestData => new() { Array.Empty<byte>() };
	}
}
