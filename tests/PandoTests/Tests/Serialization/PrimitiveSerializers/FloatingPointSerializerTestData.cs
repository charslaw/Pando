using System;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public class SingleLittleEndianSerializerTestData
{
	private static SingleLittleEndianSerializer Serializer() => new();

	public static TheoryData<float, byte[], SingleLittleEndianSerializer> SerializationTestData => new()
	{
		{ (float)Math.PI, [0xDB, 0x0F, 0x49, 0x40], Serializer() },
	};

	public static TheoryData<float, int?, SingleLittleEndianSerializer> ByteCountTestData => new()
	{
		{ 0, sizeof(float), Serializer() },
	};
}

public class DoubleLittleEndianSerializerTestData
{
	private static DoubleLittleEndianSerializer Serializer() => new();

	public static TheoryData<double, byte[], DoubleLittleEndianSerializer> SerializationTestData => new()
	{
		{ Math.PI, [0x18, 0x2D, 0x44, 0x54, 0xFB, 0x21, 0x09, 0x40], Serializer() },
	};

	public static TheoryData<double, int?, DoubleLittleEndianSerializer> ByteCountTestData => new()
	{
		{ 0, sizeof(double), Serializer() },
	};
}

// Currently unused because adding it as a MethodData results in "Ambiguous match found for 'System.Half Byte op_Explicit(System.Half)'"
public class HalfLittleEndianSerializerTestData
{
	private static HalfLittleEndianSerializer Serializer() => new();

	public static TheoryData<Half, byte[], HalfLittleEndianSerializer> SerializationTestData => new()
	{
		{ (Half)Math.PI, [0x48, 0x42], Serializer() }
	};

	public static unsafe TheoryData<Half, int?, HalfLittleEndianSerializer> ByteCountTestData => new()
	{
		{ (Half)0.0, sizeof(Half), Serializer() },
	};
}
