using System.Text;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public class AsciiStringSerializerTestData
{
	private static StringSerializer Serializer() => new(new SimpleIntSerializer(), Encoding.ASCII);

	public static TheoryData<string, byte[], StringSerializer> SerializationTestData => new()
	{
		{
			"Hello World", [
				0x00, 0x00, 0x00, 0x0B,       // length
				0x48, 0x65, 0x6C, 0x6C, 0x6F, // "Hello"
				0x20,                         // " "
				0x57, 0x6F, 0x72, 0x6C, 0x64, // "World"
			],
			Serializer()
		},
		{ "", [0x00, 0x00, 0x00, 0x00], Serializer() }
	};

	public static TheoryData<string, int?, StringSerializer> ByteCountTestData => new()
	{
		{ "", null, Serializer() },
	};
}

public class Utf8StringSerializerTestData
{
	private static StringSerializer Serializer() => new(new SimpleIntSerializer(), Encoding.UTF8);

	public static TheoryData<string, byte[], StringSerializer> SerializationTestData => new()
	{
		{
			"👋 Hello World 👋", [
				0x00, 0x00, 0x00, 0x15,       // length
				0xF0, 0x9F, 0x91, 0x8B,       // "👋"
				0x20,                         // " "
				0x48, 0x65, 0x6C, 0x6C, 0x6F, // "Hello"
				0x20,                         // " "
				0x57, 0x6F, 0x72, 0x6C, 0x64, // "World"
				0x20,                         // " "
				0xF0, 0x9F, 0x91, 0x8B,       // "👋"
			],
			Serializer()
		},
	};

	public static TheoryData<string, int?, StringSerializer> ByteCountTestData => new()
	{
		{ "", null, Serializer() },
	};
}
