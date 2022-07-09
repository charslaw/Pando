using System;
using FluentAssertions;
using GeneratedSerializers;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers;
using Pando.Serialization.PrimitiveSerializers;
using Pando.SerializerGenerator.Attributes;
using Xunit;

namespace SerializerGeneratorIntegrationTests;

[GenerateNodeSerializer]
public sealed record TestNode(object Stuff, [property: Primitive] int Value);

internal class NoopObjectSerializer : INodeSerializer<object>
{
	public int? NodeSize => null;
	public int NodeSizeForObject(object obj) => throw new NotImplementedException();
	public void Serialize(object obj, Span<byte> writeBuffer, INodeDataSink dataSink) => throw new NotImplementedException();
	public object Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource) => throw new NotImplementedException();
}

internal class NoopIntSerializer : IPrimitiveSerializer<int>
{
	public int? ByteCount => null;
	public int ByteCountForValue(int value) => throw new NotImplementedException();
	public void Serialize(int value, ref Span<byte> buffer) => throw new NotImplementedException();
	public int Deserialize(ref ReadOnlySpan<byte> buffer) => throw new NotImplementedException();
}

public class GeneratedSerializerTest
{
	public class NodeSize
	{
		[Fact]
		public void Should_return_correct_value_for_fixed_size_node()
		{
			var testNodeSerializer = new TestNodeSerializer(new NoopObjectSerializer(), new NoopIntSerializer());
			testNodeSerializer.NodeSize.Should().Be(sizeof(ulong) * 2);
		}
	}
}
