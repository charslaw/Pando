using System;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public partial class PrimitiveSerializerTests
{
	public class SingleLittleEndianSerializerTests : BaseSerializerTest<float>, ISerializerTestData<float>
	{
		protected override IPrimitiveSerializer<float> Serializer => new SingleLittleEndianSerializer();

		public static TheoryData<float, byte[]> SerializationTestData => new()
		{
			{ (float)Math.PI, new byte[] { 0xDB, 0x0F, 0x49, 0x40 } }
		};

		public static TheoryData<int?> ByteCountTestData => new() { sizeof(float) };
	}

	public class DoubleLittleEndianSerializerTests : BaseSerializerTest<double>, ISerializerTestData<double>
	{
		protected override IPrimitiveSerializer<double> Serializer => new DoubleLittleEndianSerializer();

		public static TheoryData<double, byte[]> SerializationTestData => new()
		{
			{ Math.PI, new byte[] { 0x18, 0x2D, 0x44, 0x54, 0xFB, 0x21, 0x09, 0x40 } }
		};

		public static TheoryData<int?> ByteCountTestData => new() { sizeof(double) };
	}
}
