using System;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Serialization.Collections;
using Pando.Serialization.Primitives;
using PandoTests.Utils;

namespace PandoTests.Tests.Serialization.Collections.ArraySerializerTests;

public static partial class ArraySerializerTests
{
	public class SerDes
	{
		[Test]
		public async Task Should_serialize_data_into_node_data()
		{
			int[] array = [1337, 42];

			var arraySerializer = new ArraySerializer<int>(Int32LittleEndianSerializer.Default);
			var nodeData = new SpannableList<byte>();
			var dataSource = new MemoryNodeStore(null, nodeData);

			arraySerializer.Serialize(array, stackalloc byte[8], dataSource);

			var actual = nodeData.ToArray();
			byte[] expected = [0x39, 0x5, 0, 0, 0x2A, 0, 0, 0];
			await Assert.That(actual).IsEquivalentTo(expected);
		}

		[Test]
		public async Task Should_be_able_to_deserialize_serialized_node_data()
		{
			int[] array = [1337, 42];

			var arraySerializer = new ArraySerializer<int>(Int32LittleEndianSerializer.Default);
			var dataSource = new MemoryNodeStore();

			Span<byte> hashSpan = stackalloc byte[8];
			arraySerializer.Serialize(array, hashSpan, dataSource);

			var newArray = arraySerializer.Deserialize(hashSpan, dataSource);

			await Assert.That(newArray).IsNotSameReferenceAs(array).And.IsEquivalentTo(array);
		}
	}
}
