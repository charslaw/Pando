using System;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers;

namespace SerializerGeneratorIntegrationTests.FakeSerializers;

internal class NoopObjectSerializer : INodeSerializer<object>
{
	public int? NodeSize => null;
	public int NodeSizeForObject(object obj) => throw new NotImplementedException();
	public void Serialize(object obj, Span<byte> writeBuffer, INodeDataSink dataSink) => throw new NotImplementedException();
	public object Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource) => throw new NotImplementedException();
}
