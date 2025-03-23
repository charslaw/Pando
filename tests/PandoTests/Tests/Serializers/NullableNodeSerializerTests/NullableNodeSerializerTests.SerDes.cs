using System;
using Pando.Repositories;
using Pando.Serializers;
using Pando.Vaults;
using Pando.Vaults.Utils;
using PandoTests.Utils;

namespace PandoTests.Tests.Serializers.NullableNodeSerializerTests;

public static class NullableNodeSerializerTests
{
	public class SerDes
	{
		[Test]
		public async Task Should_serialize_nonnull_value_as_node()
		{
			var value = new Vector2(1, 2);

			var nodeData = new SpannableList<byte>();
			var vault = new MemoryNodeVault(null, nodeData);
			var serializer = new NullableNodeSerializer<Vector2>(new Vector2Serializer());

			Span<byte> buffer = stackalloc byte[8];
			serializer.Serialize(value, buffer, vault);

			var actualNodeId = NodeId.FromBuffer(buffer);
			var actual = nodeData.ToArray();
			byte[] expected = [0x01, 0x02];

			using (Assert.Multiple())
			{
				await Assert.That(actualNodeId).IsNotEqualTo(NodeId.None);
				await Assert.That(actual).IsEquivalentTo(expected);
			}
		}

		[Test]
		public async Task Should_serialize_null_value_as_none()
		{
			var nodeData = new SpannableList<byte>();
			var vault = new MemoryNodeVault(null, nodeData);
			var serializer = new NullableNodeSerializer<Vector2>(new Vector2Serializer());

			Span<byte> buffer = stackalloc byte[8];
			serializer.Serialize(null, buffer, vault);

			var actualNodeId = NodeId.FromBuffer(buffer);
			var actual = nodeData.ToArray();

			using (Assert.Multiple())
			{
				await Assert.That(actualNodeId).IsEqualTo(NodeId.None);
				await Assert.That(actual).IsEmpty();
			}
		}

		[Test]
		public async Task Should_deserialize_node_as_nonnull_value()
		{
			var value = new Vector2(1, 2);

			var vault = new MemoryNodeVault();
			var serializer = new NullableNodeSerializer<Vector2>(new Vector2Serializer());

			Span<byte> buffer = stackalloc byte[8];
			serializer.Serialize(value, buffer, vault);

			var actual = serializer.Deserialize(buffer, vault);

			await Assert.That(actual).IsNotSameReferenceAs(value).And.IsEqualTo(value);
		}

		[Test]
		public async Task Should_deserialize_none_as_null()
		{
			var vault = new MemoryNodeVault();
			var serializer = new NullableNodeSerializer<Vector2>(new Vector2Serializer());

			var actual = serializer.Deserialize(NodeId.None.ToByteArray(), vault);

			await Assert.That(actual).IsNull();
		}
	}
}

file record Vector2(byte A, byte B);

file class Vector2Serializer : IPandoSerializer<Vector2>
{
	public int SerializedSize => 2;

	public void Serialize(Vector2 value, Span<byte> buffer, INodeVault nodeVault)
	{
		buffer[0] = value.A;
		buffer[1] = value.B;
	}

	public Vector2 Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault)
	{
		var a = buffer[0];
		var b = buffer[1];
		return new Vector2(a, b);
	}
}
