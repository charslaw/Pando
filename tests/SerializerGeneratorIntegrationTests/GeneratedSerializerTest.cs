using System;
using FluentAssertions;
using GeneratedSerializers;
using NSubstitute;
using Pando.Serialization.NodeSerializers;
using Pando.Serialization.PrimitiveSerializers;
using SerializerGeneratorIntegrationTests.Fakes;
using SerializerGeneratorIntegrationTests.TestSubjects;
using Xunit;

namespace SerializerGeneratorIntegrationTests;

public class GeneratedSerializerTest
{
	public class NodeSize
	{
		[Fact]
		public void Should_return_correct_value_for_fixed_size_node()
		{
			var primitiveSize = 5;

			var stuffSerializer = Substitute.For<INodeSerializer<object>>();
			var valueSerializer = Substitute.For<IPrimitiveSerializer<int>>();
			valueSerializer.ByteCount.Returns(primitiveSize);

			var serializer = new SimpleMixedNodeSerializer(stuffSerializer, valueSerializer);
			serializer.NodeSize.Should().Be(sizeof(ulong) + primitiveSize);
		}

		[Fact]
		public void Should_return_null_for_node_with_variable_size_primitive_serializer()
		{
			var stuffSerializer = Substitute.For<INodeSerializer<object>>();
			var valueSerializer = Substitute.For<IPrimitiveSerializer<int>>();

			var serializer = new SimpleMixedNodeSerializer(stuffSerializer, valueSerializer);
			serializer.NodeSize.Should().BeNull();
		}

		[Fact]
		public void Size_of_node_should_not_depend_on_size_of_child_nodes()
		{
			var stuffSerializer = Substitute.For<INodeSerializer<object>>();
			stuffSerializer.NodeSize.Returns(999_999_999);

			var valueSerializer = Substitute.For<IPrimitiveSerializer<int>>();
			valueSerializer.ByteCount.Returns(0);

			var serializer = new SimpleMixedNodeSerializer(stuffSerializer, valueSerializer);
			serializer.NodeSize.Should().Be(sizeof(ulong));
		}

		[Fact]
		public void Should_not_call_NodeSize_for_child_nodes()
		{
			var stuffSerializer = Substitute.For<INodeSerializer<object>>();
			var valueSerializer = Substitute.For<IPrimitiveSerializer<int>>();

			var serializer = new SimpleMixedNodeSerializer(stuffSerializer, valueSerializer);
			_ = serializer.NodeSize;

			_ = stuffSerializer.DidNotReceive().NodeSize;
		}

		[Fact]
		public void Should_call_ByteCount_for_child_primitives()
		{
			var stuffSerializer = Substitute.For<INodeSerializer<object>>();
			var valueSerializer = Substitute.For<IPrimitiveSerializer<int>>();

			var serializer = new SimpleMixedNodeSerializer(stuffSerializer, valueSerializer);
			_ = serializer.NodeSize;

			_ = valueSerializer.Received(1).ByteCount;
		}

		[Fact]
		public void Size_of_node_with_only_node_children_should_be_fixed()
		{
			var agesSerializer = Substitute.For<INodeSerializer<int[]>>();
			var someObjectSerializer = Substitute.For<INodeSerializer<object>>();

			var serializer = new MultipleNodeChildrenNodeSerializer(agesSerializer, someObjectSerializer);

			serializer.NodeSize.Should().Be(2 * sizeof(ulong));
		}
	}

	public class NodeSizeForObject
	{
		[Fact]
		public void Should_return_correct_fixed_size_when_contents_are_fixed_size()
		{
			var primitiveSize = 5;

			var stuffSerializer = Substitute.For<INodeSerializer<object>>();
			var valueSerializer = Substitute.For<IPrimitiveSerializer<int>>();
			valueSerializer.ByteCountForValue(Arg.Any<int>()).Returns(primitiveSize);

			var serializer = new SimpleMixedNodeSerializer(stuffSerializer, valueSerializer);

			var subject = new SimpleMixedNode(new { a = "Random", b = "Object" }, 999_999_999);
			serializer.NodeSizeForObject(subject).Should().Be(sizeof(ulong) + primitiveSize);
		}

		[Fact]
		public void Should_return_correct_value_for_node_with_variable_size_primitive()
		{
			var primitiveSize = 42;
			var stuffSerializer = Substitute.For<INodeSerializer<object>>();
			var valueSerializer = Substitute.For<IPrimitiveSerializer<int>>();
			valueSerializer.ByteCountForValue(Arg.Any<int>()).Returns(primitiveSize);

			var serializer = new SimpleMixedNodeSerializer(stuffSerializer, valueSerializer);

			var subject = new SimpleMixedNode(new { a = "Another", b = "Object" }, 5);

			serializer.NodeSizeForObject(subject).Should().Be(sizeof(ulong) + primitiveSize);
		}

		[Fact]
		public void Should_return_correct_value_for_node_with_only_node_children()
		{
			var agesSerializer = Substitute.For<INodeSerializer<int[]>>();
			var someObjectSerializer = Substitute.For<INodeSerializer<object>>();

			var serializer = new MultipleNodeChildrenNodeSerializer(agesSerializer, someObjectSerializer);

			var subject = new MultipleNodeChildrenNode(new int[999], new { a = "Just", b = "Some", c = "Object" });

			serializer.NodeSizeForObject(subject).Should().Be(2 * sizeof(ulong));
		}
	}

	public class Serialize
	{
		[Fact]
		public void Should_populate_buffer_with_child_data_when_all_children_are_primitive()
		{
			var expected = new byte[] {
				0x05, 0x00, 0x00, 0x00,       // string length 5 little endian
				0x41, 0x6c, 0x69, 0x63, 0x65, // "Alice"
				0x20, 0x00, 0x00, 0x00        // 32 little endian
			};

			var serializer = new OnlyPrimitiveChildrenNodeSerializer(StringSerializer.UTF8, Int32LittleEndianSerializer.Default);

			Span<byte> writeBuffer = stackalloc byte[expected.Length];
			var dataSink = new SpyNodeSink();
			serializer.Serialize(new OnlyPrimitiveChildrenNode("Alice", 32), writeBuffer, dataSink);

			writeBuffer.ToArray().Should().BeEquivalentTo(expected);
		}
	}
}
