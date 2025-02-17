using System;
using Pando.Serialization.Primitives;
using Xunit;

namespace PandoTests.Tests.Serialization.Primitives;

public class SingleLittleEndianSerializerTestData
{
	public static TheoryData<float, byte[], SingleLittleEndianSerializer> SerializationTestData => new()
	{
		{ (float)Math.PI, [0xDB, 0x0F, 0x49, 0x40], new SingleLittleEndianSerializer() },
	};
}

public class DoubleLittleEndianSerializerTestData
{
	public static TheoryData<double, byte[], DoubleLittleEndianSerializer> SerializationTestData => new()
	{
		{ Math.PI, [0x18, 0x2D, 0x44, 0x54, 0xFB, 0x21, 0x09, 0x40], new DoubleLittleEndianSerializer() },
	};
}

// Currently unused because adding it as a MethodData results in "Ambiguous match found for 'System.Half Byte op_Explicit(System.Half)'"
public class HalfLittleEndianSerializerTestData
{
	public static TheoryData<Half, byte[], HalfLittleEndianSerializer> SerializationTestData => new()
	{
		{ (Half)Math.PI, [0x48, 0x42], new HalfLittleEndianSerializer() }
	};
}
