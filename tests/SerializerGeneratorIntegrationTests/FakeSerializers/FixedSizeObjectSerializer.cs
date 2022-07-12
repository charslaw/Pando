using System;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers;

namespace SerializerGeneratorIntegrationTests.FakeSerializers;

public class FixedSizeObjectSerializer<T> : INodeSerializer<T>
{
	public int? NodeSize { get; }
	public int NodeSizeForObject(T obj) => NodeSize!.Value;
	public void Serialize(T obj, Span<byte> writeBuffer, INodeDataSink dataSink) => throw new NotImplementedException();
	public T Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource) => throw new NotImplementedException();

	public FixedSizeObjectSerializer(int size)
	{
		NodeSize = size;
	}
}
