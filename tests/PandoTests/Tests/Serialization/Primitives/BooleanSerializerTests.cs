using System;
using System.Collections.Generic;
using Pando.Serialization;
using Pando.Serialization.Primitives;

namespace PandoTests.Tests.Serialization.Primitives;

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
