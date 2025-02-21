using System;
using System.Text;
using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Serialization.Collections;
using Pando.Serialization.Primitives;
using PandoTests.Utils;
using Xunit;

namespace PandoTests.Tests.Serialization.Collections.StringSerializerTests;

public class SerDes
{
	public static TheoryData<string, Encoding, byte[]> SerializeData => new()
	{
		{ "Hello World", Encoding.ASCII,
			[
				0x48, 0x65, 0x6C, 0x6C, 0x6F, // "Hello"
				0x20,                         // " "
				0x57, 0x6F, 0x72, 0x6C, 0x64, // "World"
			]
		},
		{ "👋 Hello World 👋", Encoding.UTF8,
			[
				0xF0, 0x9F, 0x91, 0x8B,       // "👋"
				0x20,                         // " "
				0x48, 0x65, 0x6C, 0x6C, 0x6F, // "Hello"
				0x20,                         // " "
				0x57, 0x6F, 0x72, 0x6C, 0x64, // "World"
				0x20,                         // " "
				0xF0, 0x9F, 0x91, 0x8B,       // "👋"
			]
		}
	};

	[Theory]
	[MemberData(nameof(SerializeData))]
	public void Should_serialize_data_into_node_data(string value, Encoding encoding, byte[] expected)
	{
		var arraySerializer = new StringSerializer(encoding);
		var nodeData = new SpannableList<byte>();
		var dataSource = new MemoryNodeStore(null, nodeData);

		arraySerializer.Serialize(value, stackalloc byte[8], dataSource);

		var actual = nodeData.ToArray();

		actual.Should().BeEquivalentTo(expected);
	}

	[Theory]
	[InlineData("Hello World")]
	[InlineData("👋 Hello World 👋")]
	public void Should_be_able_to_deserialize_serialized_node_data(string value)
	{
		var stringSerializer = new StringSerializer(Encoding.UTF8);
		var dataSource = new MemoryNodeStore();

		Span<byte> hashSpan = stackalloc byte[8];
		stringSerializer.Serialize(value, hashSpan, dataSource);

		var newArray = stringSerializer.Deserialize(hashSpan, dataSource);

		newArray.Should().BeEquivalentTo(value);
	}
}
