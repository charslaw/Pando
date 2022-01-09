using Pando.Serialization.PrimitiveSerializers;
using Xunit;

// Rider doesn't detect subclasses of BaseSerializerTest as being used, because they inherit their test methods
// ReSharper disable UnusedType.Global

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public partial class PrimitiveSerializerTests
{
	public class BooleanSerializerTests : BaseSerializerTest<bool>, ISerializerTestData<bool>
	{
		protected override IPrimitiveSerializer<bool> Serializer => new BooleanSerializer();

		public static TheoryData<bool, byte[]> SerializationTestData => new()
		{
			{ true, new byte[] { 0x01 } },
			{ false, new byte[] { 0x00 } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { sizeof(bool) };
	}
}
