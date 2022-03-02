using FluentAssertions;
using Pando.SerializerGenerator.Attributes;
using SerializerGeneratorIntegrationTests.Generated;
using Xunit;

namespace SerializerGeneratorIntegrationTests;

public class GeneratedSerializerTest
{
	[GenerateNodeSerializer]
	public sealed record TestNode(object stuff, int value);

	public class NodeSize
	{
		[Fact]
		public void Should_return_correct_value_for_fixed_size_node()
		{
			var testNodeSerializer = new TestNodeSerializer(null!, null!);
			testNodeSerializer.NodeSize.Should().Be(sizeof(ulong) * 2);
		}
	}
}
