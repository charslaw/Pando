using System.Collections.Immutable;
using FluentAssertions;
using Pando.Serialization.NodeSerializers;
using Xunit;

namespace PandoTests.Tests.Serialization.NodeSerializers;

public partial class NodeSerializerTests
{
	/// Ensure consistent behavior of the "unsafe" cast of T[] to ImmutableArray
	public class ImmutableArrayBuilderTest
	{
		[Fact]
		public void Build_should_successfully_convert_array_to_immutable_array()
		{
			// call build in an inner function to simulate the ImmutableArrayBuilder going out of scope and being removed from the stack
			ImmutableArray<string> build()
			{
				using var builder = new NodeImmutableArraySerializer<string>.ImmutableArrayBuilder(5);
				builder.Add("item1");
				builder.Add("item2");
				builder.Add("item3");
				builder.Add("item4");
				builder.Add("item5");
				return builder.Build();
			}

			var actualResult = build();

			actualResult.Should().BeEquivalentTo("item1", "item2", "item3", "item4", "item5");
		}
	}
}
