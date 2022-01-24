using System;
using FluentAssertions;
using Pando.Serialization.NodeSerializers;
using Pando.Serialization.NodeSerializers.EnumerableFactory;
using PandoTests.Tests.Serialization.NodeSerializers.Utils;
using Xunit;

namespace PandoTests.Tests.Serialization.NodeSerializers;

public partial class NodeSerializerTests
{
	public class NodeListSerializerTests
	{
		public class NodeSize
		{
			[Fact]
			public void Should_return_null()
			{
				var serializer = new NodeListSerializer<object[], object>(null!, null!);
				serializer.NodeSize.Should().BeNull();
			}
		}

		public class NodeSizeForObject
		{
			[Theory]
			[InlineData(0, 0)]
			[InlineData(2, 16)]
			public void Should_return_correct_size_for_array(int elementCount, int expectedNodeSize)
			{
				var arrayFactory = new ArrayFactory<object>();
				var serializer = new NodeListSerializer<object[], object>(null!, arrayFactory);

				var array = new object[elementCount];

				var actualSize = serializer.NodeSizeForObject(array);

				actualSize.Should().Be(expectedNodeSize);
			}
		}

		public class Serialize
		{
			[Fact]
			public void Should_write_correct_node_hashes_to_write_buffer()
			{
				var elementSerializer = new FakeNodeSerializers.SerializeByLookup(
					("one", new byte[] { 1 }),
					(5, new byte[] { 5 })
				);
				var dataSink = new FakeNodeDataSources.AddNodeByLookup(
					(new byte[] { 1 }, 42),
					(new byte[] { 5 }, 43)
				);
				var arrayFactory = new ArrayFactory<object>();
				var serializer = new NodeListSerializer<object[], object>(elementSerializer, arrayFactory);

				var testData = new object[] { "one", 5 };
				byte[] writeBuffer = new byte[testData.Length * sizeof(ulong)];
				serializer.Serialize(testData, writeBuffer, dataSink);

				writeBuffer.Should()
					.BeEquivalentTo(new byte[]
						{
							42, 0, 0, 0, 0, 0, 0, 0,
							43, 0, 0, 0, 0, 0, 0, 0,
						}
					);
			}
		}

		public class Deserialize
		{
			[Fact]
			public void Should_produce_correct_results_from_hash_buffer()
			{
				var dataSource = new FakeNodeDataSources.CopyNodeBytesByLookup(
					(8000, new byte[] { 42 }),
					(10000, new byte[] { 44 })
				);
				var elementDeserializer = new FakeNodeSerializers.DeserializeByLookup(
					(new byte[] { 42 }, "item1"),
					(new byte[] { 44 }, "item2")
				);
				var arrayFactory = new ArrayFactory<object>();
				var serializer = new NodeListSerializer<object[], object>(elementDeserializer, arrayFactory);

				Span<byte> readBuffer = stackalloc byte[]
				{
					0x40, 0x1F, 0, 0, 0, 0, 0, 0, // 8000
					0x10, 0x27, 0, 0, 0, 0, 0, 0, // 10000
				};
				var actual = serializer.Deserialize(readBuffer, dataSource);

				actual.Should().BeEquivalentTo(new object[] { "item1", "item2" });
			}
		}
	}
}
