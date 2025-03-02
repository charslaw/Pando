using System;
using System.Diagnostics.CodeAnalysis;
using Pando.DataSources;
using Pando.Serialization;
using Pando.Serialization.Generic;
using Pando.Serialization.Primitives;

namespace PandoTests.Tests.Serialization.Generic.GenericNodeSerializerTests;

public static partial class GenericNodeSerializerTests
{
	public class Merge
	{
		[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local")]
		private record Node(int Value1, int Value2) : IGenericSerializable<Node, int, int>
		{
			public static Node Construct(int value1, int value2) => new(value1, value2);
		}

		[Test]
		public async Task Should_merge_two_nodes_no_conflicts()
		{
			IPandoSerializer<Node> serializer = new GenericNodeSerializer<Node, int, int>(
				Int32LittleEndianSerializer.Default,
				Int32LittleEndianSerializer.Default
			);
			var dataSource = new MemoryNodeStore();

			Span<byte> buffer = stackalloc byte[sizeof(ulong) * 3];

			var baseBuffer = buffer.Slice(0, sizeof(ulong));
			var targetBuffer = buffer.Slice(sizeof(ulong), sizeof(ulong));
			var sourceBuffer = buffer.Slice(sizeof(ulong) * 2, sizeof(ulong));

			serializer.Serialize(new Node(100, 100), baseBuffer, dataSource);
			serializer.Serialize(new Node(200, 100), targetBuffer, dataSource);
			serializer.Serialize(new Node(100, 300), sourceBuffer, dataSource);

			serializer.Merge(baseBuffer, targetBuffer, sourceBuffer, dataSource);

			var actual = serializer.Deserialize(baseBuffer, dataSource);

			await Assert.That(actual).IsEqualTo(new Node(200, 300));
		}

		[Test]
		public async Task Should_merge_two_nodes_with_conflict()
		{
			IPandoSerializer<Node> serializer = new GenericNodeSerializer<Node, int, int>(
				Int32LittleEndianSerializer.Default,
				Int32LittleEndianSerializer.Default
			);
			var dataSource = new MemoryNodeStore();

			Span<byte> buffer = stackalloc byte[sizeof(ulong) * 3];

			var baseBuffer = buffer.Slice(0, sizeof(ulong));
			var targetBuffer = buffer.Slice(sizeof(ulong), sizeof(ulong));
			var sourceBuffer = buffer.Slice(sizeof(ulong) * 2, sizeof(ulong));

			serializer.Serialize(new Node(100, 100), baseBuffer, dataSource);
			serializer.Serialize(new Node(200, 300), targetBuffer, dataSource);
			serializer.Serialize(new Node(400, 100), sourceBuffer, dataSource);

			serializer.Merge(baseBuffer, targetBuffer, sourceBuffer, dataSource);

			var actual = serializer.Deserialize(baseBuffer, dataSource);

			await Assert.That(actual).IsEqualTo(new Node(400, 300));
		}
	}
}
