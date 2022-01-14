using FluentAssertions;
using Pando.Serialization;
using Pando.Serialization.PrimitiveSerializers;
using PandoTests.Tests.Serialization.PrimitiveSerializers;
using PandoTests.Utils;
using Xunit;

namespace PandoTests.Tests.Serialization;

public class PrimitiveArrayNodeSerializerTests
{
	public class NodeSize
	{
		[Fact]
		public void Should_return_null()
		{
			var elementSerializer = new SimpleIntSerializer();
			var nodeSerializer = new PrimitiveArrayNodeSerializer<int>(elementSerializer);
			nodeSerializer.NodeSize.Should().BeNull();
		}
	}

	public class Serialize
	{
		[Fact]
		public void Should_write_correct_node_contents_when_elements_are_fixed_size()
		{
			var elementSerializer = new SimpleIntSerializer();
			var nodeSerializer = new PrimitiveArrayNodeSerializer<int>(elementSerializer);
			var dataSink = new NodeDataSinkSpy();

			nodeSerializer.Serialize(new[] { 0, -42 }, dataSink);

			dataSink.ReceivedNodeBytes.Should()
				.BeEquivalentTo(new object[]
					{
						new byte[] { 0, 0, 0, 0, 0xFF, 0xFF, 0xFF, 0xD6 }
					}
				);
		}

		[Fact]
		public void Should_write_correct_node_contents_when_elements_are_variable_size()
		{
			var elementSerializer = new NullableSerializer<int>(new SimpleIntSerializer());
			var nodeSerializer = new PrimitiveArrayNodeSerializer<int?>(elementSerializer);
			var dataSink = new NodeDataSinkSpy();

			nodeSerializer.Serialize(new int?[] { null, -42 }, dataSink);

			dataSink.ReceivedNodeBytes.Should()
				.BeEquivalentTo(new object[]
					{
						new byte[] { 0, 1, 0xFF, 0xFF, 0xFF, 0xD6 }
					}
				);
		}
	}

	public class Deserialize
	{
		[Fact]
		public void Should_read_correct_node_contents_when_elements_are_fixed_size()
		{
			var elementSerializer = new SimpleIntSerializer();
			var nodeSerializer = new PrimitiveArrayNodeSerializer<int>(elementSerializer);

			var actual = nodeSerializer.Deserialize(new byte[] { 0, 0, 0, 0, 0xFF, 0xFF, 0xFF, 0xD6 }, null!);

			actual.Should().BeEquivalentTo(new[] { 0, -42 });
		}

		[Fact]
		public void Should_read_correct_node_contents_when_elements_are_variable_size()
		{
			var elementSerializer = new NullableSerializer<byte>(new ByteSerializer());
			var nodeSerializer = new PrimitiveArrayNodeSerializer<byte?>(elementSerializer);

			var actual = nodeSerializer.Deserialize(new byte[]
				{
					0,
					1, 42,
					0,
					1, 123,
					0,
					1, 1,
					1, 2,
					1, 3,
					1, 4,
					0,
					1, 5,
					1, 6,
					1, 7,
					1, 8,
					0,
					1, 9,
					1, 10,
				}, null!
			);

			actual.Should().BeEquivalentTo(new int?[] { null, 42, null, 123, null, 1, 2, 3, 4, null, 5, 6, 7, 8, null, 9, 10 });
		}
	}
}
