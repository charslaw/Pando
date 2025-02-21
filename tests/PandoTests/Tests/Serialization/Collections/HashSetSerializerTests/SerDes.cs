using System;
using System.Collections.Generic;
using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Serialization.Collections;
using Pando.Serialization.Primitives;
using PandoTests.Utils;
using Xunit;

namespace PandoTests.Tests.Serialization.Collections.HashSetSerializerTests;

public class SerDes
{
	[Fact]
	public void Should_serialize_data_into_node_data()
	{
		HashSet<int> array = [1337, 42];

		var setSerializer = new HashSetSerializer<int>(Int32LittleEndianSerializer.Default);
		var nodeData = new SpannableList<byte>();
		var dataSource = new MemoryNodeStore(null, nodeData);

		setSerializer.Serialize(array, stackalloc byte[8], dataSource);

		var actual = nodeData.ToArray();
		byte[] expected = [0x39, 0x5, 0, 0, 0x2A, 0, 0, 0];
		actual.Should().BeEquivalentTo(expected);
	}

	[Fact]
	public void Should_be_able_to_deserialize_serialized_node_data()
	{
		HashSet<int> array = [1337, 42];

		var setSerializer = new HashSetSerializer<int>(Int32LittleEndianSerializer.Default);
		var dataSource = new MemoryNodeStore();

		Span<byte> hashSpan = stackalloc byte[8];
		setSerializer.Serialize(array, hashSpan, dataSource);

		var newArray = setSerializer.Deserialize(hashSpan, dataSource);

		newArray.Should().NotBeSameAs(array).And.BeEquivalentTo(array);
	}
}
