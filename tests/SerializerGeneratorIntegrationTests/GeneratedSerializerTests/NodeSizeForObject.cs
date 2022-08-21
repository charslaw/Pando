using FluentAssertions;
using GeneratedSerializers;
using NSubstitute;
using Pando.Serialization.NodeSerializers;
using Pando.Serialization.PrimitiveSerializers;
using SerializerGeneratorIntegrationTests.TestSubjects;
using Xunit;

namespace SerializerGeneratorIntegrationTests.GeneratedSerializerTests;

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
