using Pando.Serialization.PrimitiveSerializers;
using Xunit;

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public class BooleanSerializerTestData
{
	public static TheoryData<bool, byte[], BooleanSerializer> SerializationTestData => new()
	{
		{ true, [0x01], new BooleanSerializer() },
		{ false, [0x00], new BooleanSerializer() },
	};

	public static TheoryData<bool, int?, BooleanSerializer> ByteCountTestData => new() { { false, sizeof(bool), new BooleanSerializer() } };
}
