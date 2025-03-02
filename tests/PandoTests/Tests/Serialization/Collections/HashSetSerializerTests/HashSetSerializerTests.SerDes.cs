using System;
using System.Collections.Generic;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Serialization.Collections;
using Pando.Serialization.Primitives;
using PandoTests.Utils;

namespace PandoTests.Tests.Serialization.Collections.HashSetSerializerTests;

public static class HashSetSerializerTests
{
	public class SerDes
	{
		[Test]
		public async Task Should_serialize_data_into_node_data()
		{
			HashSet<int> array = [1337, 42];

			var setSerializer = new HashSetSerializer<int>(Int32LittleEndianSerializer.Default);
			var nodeData = new SpannableList<byte>();
			var dataSource = new MemoryNodeStore(null, nodeData);

			setSerializer.Serialize(array, stackalloc byte[8], dataSource);

			var actual = nodeData.ToArray();
			byte[] expected = [0x39, 0x5, 0, 0, 0x2A, 0, 0, 0];
			await Assert.That(actual).IsEquivalentTo(expected);
		}

		[Test]
		public async Task Should_be_able_to_deserialize_serialized_node_data()
		{
			HashSet<int> set = [1337, 42];

			var setSerializer = new HashSetSerializer<int>(Int32LittleEndianSerializer.Default);
			var dataSource = new MemoryNodeStore();

			Span<byte> hashSpan = stackalloc byte[8];
			setSerializer.Serialize(set, hashSpan, dataSource);

			var newSet = setSerializer.Deserialize(hashSpan, dataSource);

			await Assert.That(newSet).IsNotSameReferenceAs(set).And.IsEquivalentTo(set);
		}
	}
}
