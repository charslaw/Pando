using System;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers;

namespace SerializerGeneratorIntegrationTests.Fakes;

public class NoOpNodeSerializer<T> : INodeSerializer<T>
{
	public int? NodeSize => default;
	public int NodeSizeForObject(T obj) => default;
	public void Serialize(T obj, Span<byte> writeBuffer, INodeDataSink dataSink) {  }
	public T Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource) { throw new NotImplementedException(); }
}
