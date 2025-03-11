using System;
using System.Diagnostics.CodeAnalysis;
using Pando.Serializers.Generic;
using Pando.Serializers.Primitives;
using Pando.Vaults;
using Pando.Vaults.Utils;
using PandoTests.Utils;

namespace PandoTests.Tests.Serializers.Generic.GenericNodeSerializerTests;

[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local")]
file record Pair(int A, int B) : IGenericSerializable<Pair, int, int>
{
	public static Pair Construct(int a, int b) => new(a, b);
}

public static partial class GenericNodeSerializerTests
{
	public class SerDes
	{
		[Test]
		public async Task Should_serialize_primitive_children_into_node_data()
		{
			var pair = new Pair(1337, 42);

			var pairSerializer = new GenericNodeSerializer<Pair, int, int>(
				Int32LittleEndianSerializer.Default,
				Int32LittleEndianSerializer.Default
			);
			var nodeData = new SpannableList<byte>();
			var vault = new MemoryNodeVault(null, nodeData);

			pairSerializer.Serialize(pair, stackalloc byte[8], vault);

			var actual = nodeData.ToArray();
			byte[] expected = [0x39, 0x5, 0, 0, 0x2A, 0, 0, 0];
			await Assert.That(actual).IsEquivalentTo(expected);
		}

		[Test]
		public async Task Should_be_able_to_deserialize_serialized_node_data()
		{
			var pair = new Pair(1337, 42);

			var pairSerializer = new GenericNodeSerializer<Pair, int, int>(
				Int32LittleEndianSerializer.Default,
				Int32LittleEndianSerializer.Default
			);
			var vault = new MemoryNodeVault();

			Span<byte> hashSpan = stackalloc byte[8];
			pairSerializer.Serialize(pair, hashSpan, vault);

			var newPair = pairSerializer.Deserialize(hashSpan, vault);

			await Assert.That(newPair).IsNotSameReferenceAs(pair).And.IsEqualTo(pair);
		}
	}
}
