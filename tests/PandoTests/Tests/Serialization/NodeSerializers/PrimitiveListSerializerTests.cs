using FluentAssertions;
using Pando.Serialization.NodeSerializers;
using Pando.Serialization.NodeSerializers.EnumerableFactory;
using Pando.Serialization.PrimitiveSerializers;
using PandoTests.Tests.Serialization.PrimitiveSerializers;
using Xunit;

namespace PandoTests.Tests.Serialization.NodeSerializers;

public partial class NodeSerializerTests
{
	public class PrimitiveListSerializerTests
	{
		public class NodeSize
		{
			[Fact]
			public void Should_return_null()
			{
				var serializer = new PrimitiveListSerializer<object[], object>(null!, null!);
				serializer.NodeSize.Should().BeNull();
			}
		}

		public class NodeSizeForObject
		{
			[Theory]
			[InlineData(new int[] { }, 0)]
			[InlineData(new[] { 1, 2 }, 8)]
			public void Should_return_correct_size_for_array(int[] array, int expectedSize)
			{
				var elementSerializer = new SimpleIntSerializer();
				var arrayFactory = new ArrayFactory<int>();
				var serializer = new PrimitiveListSerializer<int[], int>(elementSerializer, arrayFactory);
				var actualSize = serializer.NodeSizeForObject(array);

				actualSize.Should().Be(expectedSize);
			}
		}

		public class Serialize
		{
			[Fact]
			public void Should_write_correct_node_contents_when_elements_are_fixed_size()
			{
				var elementSerializer = new SimpleIntSerializer();
				var arrayFactory = new ArrayFactory<int>();
				var serializer = new PrimitiveListSerializer<int[], int>(elementSerializer, arrayFactory);

				var writeBuffer = new byte[sizeof(int) * 2];
				serializer.Serialize(new[] { 0, -42 }, writeBuffer, null!);

				writeBuffer.Should().BeEquivalentTo(new byte[] { 0, 0, 0, 0, 0xFF, 0xFF, 0xFF, 0xD6 });
			}

			[Fact]
			public void Should_write_correct_node_contents_when_elements_are_variable_size()
			{
				var elementSerializer = new NullableSerializer<int>(new SimpleIntSerializer());
				var arrayFactory = new ArrayFactory<int?>();
				var serializer = new PrimitiveListSerializer<int?[], int?>(elementSerializer, arrayFactory);

				var writeBuffer = new byte[6];
				serializer.Serialize(new int?[] { null, -42 }, writeBuffer, null!);

				writeBuffer.Should().BeEquivalentTo(new byte[] { 0, 1, 0xFF, 0xFF, 0xFF, 0xD6 });
			}
		}

		public class Deserialize
		{
			[Fact]
			public void Should_read_correct_node_contents_when_elements_are_fixed_size()
			{
				var elementSerializer = new SimpleIntSerializer();
				var arrayFactory = new ArrayFactory<int>();
				var serializer = new PrimitiveListSerializer<int[], int>(elementSerializer, arrayFactory);

				var actual = serializer.Deserialize(new byte[] { 0, 0, 0, 0, 0xFF, 0xFF, 0xFF, 0xD6 }, null!);

				actual.Should().BeEquivalentTo(new[] { 0, -42 });
			}

			[Fact]
			public void Should_read_correct_node_contents_when_elements_are_variable_size()
			{
				var elementSerializer = new NullableSerializer<byte>(new ByteSerializer());
				var arrayFactory = new ArrayFactory<byte?>();
				var serializer = new PrimitiveListSerializer<byte?[], byte?>(elementSerializer, arrayFactory);

				var actual = serializer.Deserialize(new byte[]
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

				actual.Should().BeEquivalentTo(new byte?[] { null, 42, null, 123, null, 1, 2, 3, 4, null, 5, 6, 7, 8, null, 9, 10 });
			}
		}
	}
}
