using FluentAssertions;
using GeneratedSerializers;
using SerializerGeneratorIntegrationTests.FakeSerializers;
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
			var testNodeSerializer = new SimpleMixedNodeSerializer(new NoopObjectSerializer(), new FixedSizePrimitiveSerializer<int>(primitiveSize));
			testNodeSerializer.NodeSize.Should().Be(sizeof(ulong) + primitiveSize);
		}

		[Fact]
		public void Should_return_null_for_node_with_variable_size_primitive_serializer()
		{
			var testNodeSerializer = new SimpleMixedNodeSerializer(new NoopObjectSerializer(), new NoopIntSerializer());
			testNodeSerializer.NodeSize.Should().BeNull();
		}

		[Fact]
		public void Size_of_node_should_not_depend_on_size_of_child_nodes()
		{
			var testNodeSerializer = new SimpleMixedNodeSerializer(
				new FixedSizeObjectSerializer<object>(42),
				new FixedSizePrimitiveSerializer<int>(0)
			);
			testNodeSerializer.NodeSize.Should().Be(sizeof(ulong));
		}
	}

	public class NodeSizeForObject
	{
		[Fact]
		public void Should_return_correct_fixed_size_when_contents_are_fixed_size()
		{
			var primitiveSize = 5;
			var testNodeSerializer = new SimpleMixedNodeSerializer(new NoopObjectSerializer(), new FixedSizePrimitiveSerializer<int>(primitiveSize));
			var subject = new SimpleMixedNode(new { a = "Random", b = "Object" }, 42);
			testNodeSerializer.NodeSizeForObject(subject).Should().Be(sizeof(ulong) + primitiveSize);
		}

		[Fact]
		public void Should_return_correct_value_for_node_with_variable_size_primitive()
		{
			var primitiveSize = 42;
			var testNodeSerializer = new SimpleMixedNodeSerializer(new NoopObjectSerializer(), new RelaySizeIntSerializer());
			var subject = new SimpleMixedNode(new { a = "Random", b = "Object" }, primitiveSize);
			testNodeSerializer.NodeSizeForObject(subject).Should().Be(sizeof(ulong) + primitiveSize);
		}
	}
}
