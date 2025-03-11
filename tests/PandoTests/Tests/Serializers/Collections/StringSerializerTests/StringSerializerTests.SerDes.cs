using System;
using System.Collections.Generic;
using System.Text;
using Pando.Serializers.Collections;
using Pando.Vaults;
using Pando.Vaults.Utils;
using PandoTests.Utils;

namespace PandoTests.Tests.Serializers.Collections.StringSerializerTests;

public static class StringSerializerTests
{
	public class SerDes
	{
		public static IEnumerable<Func<(string, Encoding, byte[])>> SerializeData()
		{
			// csharpier-ignore-start
			yield return () =>
				(
					"Hello World",
					Encoding.ASCII,
					[
						0x48, 0x65, 0x6C, 0x6C, 0x6F, // "Hello"
						0x20,                         // " "
						0x57, 0x6F, 0x72, 0x6C, 0x64, // "World"
					]
				);
			yield return () =>
				(
					"👋 Hello World 👋",
					Encoding.UTF8,
					[
						0xF0, 0x9F, 0x91, 0x8B,       // "👋"
						0x20,                         // " "
						0x48, 0x65, 0x6C, 0x6C, 0x6F, // "Hello"
						0x20,                         // " "
						0x57, 0x6F, 0x72, 0x6C, 0x64, // "World"
						0x20,                         // " "
						0xF0, 0x9F, 0x91, 0x8B,       // "👋"
					]
				);
			// csharpier-ignore-end
		}

		[Test]
		[MethodDataSource(nameof(SerializeData))]
		public async Task Should_serialize_data_into_node_data(string value, Encoding encoding, byte[] expected)
		{
			var arraySerializer = new StringSerializer(encoding);
			var nodeData = new SpannableList<byte>();
			var vault = new MemoryNodeVault(null, nodeData);

			arraySerializer.Serialize(value, stackalloc byte[8], vault);

			var actual = nodeData.ToArray();

			await Assert.That(actual).IsEquivalentTo(expected);
		}

		[Test]
		[Arguments("Hello World")]
		[Arguments("👋 Hello World 👋")]
		public async Task Should_be_able_to_deserialize_serialized_node_data(string value)
		{
			var stringSerializer = new StringSerializer(Encoding.UTF8);
			var vault = new MemoryNodeVault();

			Span<byte> hashSpan = stackalloc byte[8];
			stringSerializer.Serialize(value, hashSpan, vault);

			var newString = stringSerializer.Deserialize(hashSpan, vault);

			await Assert.That(newString).IsEquivalentTo(value);
		}
	}
}
