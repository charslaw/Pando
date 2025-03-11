using System;
using System.Collections.Generic;
using Pando.Serializers;
using Pando.Serializers.Primitives;

namespace PandoTests.Tests.Serializers.Primitives;

[InheritsTests]
public class BooleanSerializerTests : PrimitiveSerializerTest<bool>
{
	public override IPandoSerializer<bool> CreateSerializer() => new BooleanSerializer();

	public override IEnumerable<Func<(bool, byte[])>> SerializationTestData()
	{
		yield return () => (true, [0x01]);
		yield return () => (false, [0x00]);
	}
}
