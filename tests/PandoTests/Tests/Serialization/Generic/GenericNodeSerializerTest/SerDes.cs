using System;
using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Serialization.Generic;
using Pando.Serialization.Primitives;
using PandoTests.Utils;
using Xunit;

namespace PandoTests.Tests.Serialization.Generic.GenericNodeSerializerTest;

file record Pair(int A, int B) : IGenericSerializable<Pair, int, int>
{
	public static Pair Construct(int a, int b) => new(a, b);
}

public class SerDes
{
	[Fact]
	public void Should_serialize_primitive_children_into_node_data()
	{
		var pair = new Pair(1337, 42);

		var pairSerializer = new GenericNodeSerializer<Pair, int, int>(Int32LittleEndianSerializer.Default, Int32LittleEndianSerializer.Default);
		var nodeData = new SpannableList<byte>();
		var dataSource = new MemoryDataSource(null, null, nodeData);

		pairSerializer.Serialize(pair, stackalloc byte[8], dataSource);

		var actual = nodeData.ToArray();
		byte[] expected = [0x39, 0x5, 0, 0, 0x2A, 0, 0, 0];
		actual.Should().BeEquivalentTo(expected);
	}

	[Fact]
	public void Should_be_able_to_deserialize_serialized_node_data()
	{
		var pair = new Pair(1337, 42);

		var pairSerializer = new GenericNodeSerializer<Pair, int, int>(Int32LittleEndianSerializer.Default, Int32LittleEndianSerializer.Default);
		var dataSource = new MemoryDataSource();

		Span<byte> hashSpan = stackalloc byte[8];
		pairSerializer.Serialize(pair, hashSpan, dataSource);

		var newPair = pairSerializer.Deserialize(hashSpan, dataSource);

		newPair.Should().NotBeSameAs(pair).And.BeEquivalentTo(pair);
	}
}
