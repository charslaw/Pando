using System.Text;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

// Rider doesn't detect subclasses of BaseSerializerTest as being used, because they inherit their test methods
// ReSharper disable UnusedType.Global

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public partial class PrimitiveSerializerTests
{
	public class AsciiStringSerializerTests : BasePrimitiveSerializerTest<string>, ISerializerTestData<string>
	{
		protected override IPrimitiveSerializer<string> Serializer => new StringSerializer(new SimpleIntSerializer(), Encoding.ASCII);

		public static TheoryData<string, byte[]> SerializationTestData => new()
		{
			{
				"Hello World", new byte[]
				{
					0x00, 0x00, 0x00, 0x0B,       // Length
					0x48, 0x65, 0x6C, 0x6C, 0x6F, // "Hello"
					0x20,                         // " "
					0x57, 0x6F, 0x72, 0x6C, 0x64  // "World
				}
			},
			{ "", new byte[] { 0x00, 0x00, 0x00, 0x00 } }
		};

		public static TheoryData<int?> ByteCountTestData => new() { null };
	}

	public class Utf8StringSerializerTests : BasePrimitiveSerializerTest<string>, ISerializerTestData<string>
	{
		protected override IPrimitiveSerializer<string> Serializer => new StringSerializer(new SimpleIntSerializer(), Encoding.UTF8);

		public static TheoryData<string, byte[]> SerializationTestData => new()
		{
			{
				"👋 Hello World 👋", new byte[]
				{
					0x00, 0x00, 0x00, 0x15,       // length
					0xF0, 0x9F, 0x91, 0x8B,       // "👋"
					0x20,                         // " "
					0x48, 0x65, 0x6C, 0x6C, 0x6F, // "Hello"
					0x20,                         // " "
					0x57, 0x6F, 0x72, 0x6C, 0x64, // "World
					0x20,                         // " "
					0xF0, 0x9F, 0x91, 0x8B,       // "👋"
				}
			},
		};

		public static TheoryData<int?> ByteCountTestData => new() { null };
	}
}
