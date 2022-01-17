using System;
using Pando.DataSources;

namespace Pando.Serialization;

public interface INodeSerializer<T>
{
	/// The size in bytes of the data produced by this serializer, if known.
	/// If the size can be dynamic, should return null.
	/// <remarks>This should be constant after initialization. If the size of the data can vary, return null</remarks>
	int? NodeSize { get; }

	/// Returns the size of the buffer required to serialize the given object.
	int NodeSizeForObject(T obj);

	void Serialize(T obj, Span<byte> writeBuffer, INodeDataSink dataSink);

	T Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource);
}
