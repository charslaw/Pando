using Pando.Serialization.Primitives;
using Xunit;

namespace PandoTests.Tests.Serialization.Primitives;

public class BooleanSerializerTestData
{
	public static TheoryData<bool, byte[], BooleanSerializer> SerializationTestData => new()
	{
		{ true, [0x01], new BooleanSerializer() },
		{ false, [0x00], new BooleanSerializer() },
	};
}
