using System;
using System.Collections.Generic;
using Pando.Serialization.Collections;
using Pando.Serialization.Primitives;
using Pando.Vaults;
using Pando.Vaults.Utils;
using PandoTests.Utils;

namespace PandoTests.Tests.Serialization.Collections.ListSerializerTests;

public static class ListSerializerTests
{
	public class SerDes
	{
		[Test]
		public async Task Should_serialize_data_into_node_data()
		{
			List<int> list = [1337, 42];

			var listSerializer = new ListSerializer<int>(Int32LittleEndianSerializer.Default);
			var nodeData = new SpannableList<byte>();
			var vault = new MemoryNodeVault(null, nodeData);

			listSerializer.Serialize(list, stackalloc byte[8], vault);

			var actual = nodeData.ToArray();
			byte[] expected = [0x39, 0x5, 0, 0, 0x2A, 0, 0, 0];
			await Assert.That(actual).IsEquivalentTo(expected);
		}

		[Test]
		public async Task Should_be_able_to_deserialize_serialized_node_data()
		{
			List<int> list = [1337, 42];

			var listSerializer = new ListSerializer<int>(Int32LittleEndianSerializer.Default);
			var vault = new MemoryNodeVault();

			Span<byte> hashSpan = stackalloc byte[8];
			listSerializer.Serialize(list, hashSpan, vault);

			var newList = listSerializer.Deserialize(hashSpan, vault);

			await Assert.That(newList).IsNotSameReferenceAs(list).And.IsEquivalentTo(list);
		}
	}
}
