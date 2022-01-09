using System;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

// Rider doesn't detect subclasses of BaseSerializerTest as being used, because they inherit their test methods
// ReSharper disable UnusedType.Global

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

	public class HalfLittleEndianSerializerTests : BaseSerializerTest<Half>, ISerializerTestData<Half>
	{
		protected override IPrimitiveSerializer<Half> Serializer => new HalfLittleEndianSerializer();

		public static TheoryData<Half, byte[]> SerializationTestData => new()
		{
			{ (Half)Math.PI, new byte[] { 0x48, 0x42 } }
		};

		public static unsafe TheoryData<int?> ByteCountTestData => new() { sizeof(Half) };
	}
}
