using System;
using System.Collections.Generic;
using Pando.Serialization;
using Pando.Serialization.Primitives;

namespace PandoTests.Tests.Serialization.Primitives;

[InheritsTests]
public class SingleLittleEndianSerializerTests : PrimitiveSerializerTest<float>
{
	public override IPandoSerializer<float> CreateSerializer() => new SingleLittleEndianSerializer();

	public override IEnumerable<Func<(float, byte[])>> SerializationTestData()
	{
		yield return () => (MathF.PI, [0xDB, 0x0F, 0x49, 0x40]);
	}
}

[InheritsTests]
public class DoubleLittleEndianSerializerTests : PrimitiveSerializerTest<double>
{
	public override IPandoSerializer<double> CreateSerializer() => new DoubleLittleEndianSerializer();

	public override IEnumerable<Func<(double, byte[])>> SerializationTestData()
	{
		yield return () => (Math.PI, [0x18, 0x2D, 0x44, 0x54, 0xFB, 0x21, 0x09, 0x40]);
	}
}

[InheritsTests]
public class HalfLittleEndianSerializerTests : PrimitiveSerializerTest<Half>
{
	public override IPandoSerializer<Half> CreateSerializer() => new HalfLittleEndianSerializer();

	public override IEnumerable<Func<(Half, byte[])>> SerializationTestData()
	{
		yield return () => ((Half)Math.PI, [0x48, 0x42]);
	}
}
