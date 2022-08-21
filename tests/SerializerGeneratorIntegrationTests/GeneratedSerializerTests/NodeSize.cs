using FluentAssertions;
using GeneratedSerializers;
using NSubstitute;
using Pando.Serialization.NodeSerializers;
using Pando.Serialization.PrimitiveSerializers;
using Xunit;

namespace SerializerGeneratorIntegrationTests.GeneratedSerializerTests;

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
