using System;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

// Rider doesn't detect subclasses of BaseSerializerTest as being used, because they inherit their test methods
// ReSharper disable UnusedType.Global

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public class NumericSerializerTest
{
	public class ByteSerializerTest : BaseSerializerTest<byte>, ISerializerTestData<byte>
	{
		private const int EXPECTED_SIZE = sizeof(byte);

		protected override IPrimitiveSerializer<byte> Serializer => new ByteSerializer();

		public static TheoryData<byte, byte[]> SerializationTestData => new()
		{
			{ 0, new byte[] { 0x00 } },
			{ 127, new byte[] { 0x7F } },
			{ 128, new byte[] { 0x80 } },
			{ 255, new byte[] { 0xFF } }
		};

		public static TheoryData<byte, int> SerializeUndersizedBufferTestData => new() { { 255, EXPECTED_SIZE } };
		public static TheoryData<byte[]> DeserializeUndersizedBufferTestData => new() { Array.Empty<byte>() };

		public static TheoryData<int?> ByteCountTestData => new() { EXPECTED_SIZE };
		public static TheoryData<byte, int> ByteCountForValueTestData => new() { { 128, EXPECTED_SIZE } };
	}
}
