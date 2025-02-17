using System;
using System.Collections.Generic;
using FluentAssertions;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Serialization.Collections;
using Pando.Serialization.Primitives;
using PandoTests.Utils;
using Xunit;

namespace PandoTests.Tests.Serialization.Collections.ListSerializerTests;

public class SerDes
{
	[Fact]
	public void Should_serialize_data_into_node_data()
	{
		List<int> array = [1337, 42];

		var listSerializer = new ListSerializer<int>(Int32LittleEndianSerializer.Default);
		var nodeData = new SpannableList<byte>();
		var dataSource = new MemoryDataSource(null, null, nodeData);

		listSerializer.Serialize(array, stackalloc byte[8], dataSource);

		var actual = nodeData.ToArray();
		byte[] expected = [0x39, 0x5, 0, 0, 0x2A, 0, 0, 0];
		actual.Should().BeEquivalentTo(expected);
	}

	[Fact]
	public void Should_be_able_to_deserialize_serialized_node_data()
	{
		List<int> list = [1337, 42];

		var listSerializer = new ListSerializer<int>(Int32LittleEndianSerializer.Default);
		var dataSource = new MemoryDataSource();

		Span<byte> hashSpan = stackalloc byte[8];
		listSerializer.Serialize(list, hashSpan, dataSource);

		var newArray = listSerializer.Deserialize(hashSpan, dataSource);

		newArray.Should().NotBeSameAs(list).And.BeEquivalentTo(list);
	}
}
