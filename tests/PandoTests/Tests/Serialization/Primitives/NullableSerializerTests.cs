using System;
using System.Collections.Generic;
using Pando.Serialization;
using Pando.Serialization.Primitives;

namespace PandoTests.Tests.Serialization.Primitives;

[InheritsTests]
public class NullableSerializerTests : PrimitiveSerializerTest<byte?>
{
	public override IPandoSerializer<byte?> CreateSerializer() =>
		new NullableSerializer<byte>(new ByteSerializer());

	public override IEnumerable<Func<(byte?, byte[])>> SerializationTestData()
	{
		yield return () => (null, [0, 0]);
		yield return () => (byte.MaxValue, [0x01, 0xFF]);
	}
}
