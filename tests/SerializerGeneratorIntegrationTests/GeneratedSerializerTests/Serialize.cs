using System;
using FluentAssertions;
using GeneratedSerializers;
using Pando.Serialization.PrimitiveSerializers;
using SerializerGeneratorIntegrationTests.Fakes;
using SerializerGeneratorIntegrationTests.TestSubjects;
using Xunit;

namespace SerializerGeneratorIntegrationTests.GeneratedSerializerTests;

public class Serialize
{
	[Fact]
	public void Should_populate_buffer_with_child_data_when_all_children_are_primitive()
	{
		var expected = new byte[]
		{
			0x05, 0x00, 0x00, 0x00,       // string length, 5, little endian
			0x41, 0x6c, 0x69, 0x63, 0x65, // "Alice"
			0x20, 0x00, 0x00, 0x00        // 32, little endian
		};

		var serializer = new OnlyPrimitiveChildrenNodeSerializer(StringSerializer.UTF8, Int32LittleEndianSerializer.Default);

		Span<byte> writeBuffer = stackalloc byte[expected.Length];
		var dataSink = new SpyNodeSink();
		serializer.Serialize(new OnlyPrimitiveChildrenNode("Alice", 32), writeBuffer, dataSink);

		writeBuffer.ToArray().Should().BeEquivalentTo(expected);
	}

	[Fact]
	public void Should_populate_buffer_with_child_node_hash_and_child_primitive_value()
	{
		var expected = new byte[]
		{
			0xD2, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // node hash, 1234, little endian
			0xDB, 0x03, 0x00, 0x00                          // 987, little endian
		};

		var serializer = new SimpleMixedNodeSerializer(new NoOpNodeSerializer<object>(), Int32LittleEndianSerializer.Default);

		Span<byte> writeBuffer = stackalloc byte[expected.Length];
		var dataSink = new SequentialHashNodeSink(1234UL);
		serializer.Serialize(new SimpleMixedNode(new { a = "a" }, 987), writeBuffer, dataSink);

		writeBuffer.ToArray().Should().BeEquivalentTo(expected);
	}

	[Fact]
	public void Should_populate_buffer_with_children_node_hashes()
	{
		var expected = new byte[]
		{
			0xD2, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // node hash, 1234, little endian
			0xD3, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00  // node hash, 1235, little endian
		};

		var serializer = new MultipleNodeChildrenNodeSerializer(new NoOpNodeSerializer<int[]>(), new NoOpNodeSerializer<object>());

		Span<byte> writeBuffer = stackalloc byte[expected.Length];
		var dataSink = new SequentialHashNodeSink(1234UL);
		serializer.Serialize(new MultipleNodeChildrenNode(new[] { 1, 2, 3 }, new { a = "a" }), writeBuffer, dataSink);

		writeBuffer.ToArray().Should().BeEquivalentTo(expected);
	}

	[Fact]
	public void Should_serialize_child_nodes_to_data_sink()
	{
		var serializer = new MultipleNodeChildrenNodeSerializer(new NoOpNodeSerializer<int[]>(), new NoOpNodeSerializer<object>());

		Span<byte> writeBuffer = stackalloc byte[sizeof(ulong) * 2];
		var dataSink = new SpyNodeSink();
		serializer.Serialize(new MultipleNodeChildrenNode(new[] { 1, 2, 3 }, new { a = "a" }), writeBuffer, dataSink);

		dataSink.AddNodeCallCount.Should().Be(2);
	}
}
