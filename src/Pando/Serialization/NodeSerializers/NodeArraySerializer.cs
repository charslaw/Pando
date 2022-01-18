using System;
using Pando.DataSources;
using Pando.DataSources.Utils;

namespace Pando.Serialization.NodeSerializers;

/// Serializes a node that is an array of nodes using the given node serializer.
public class NodeArraySerializer<T> : INodeSerializer<T[]>
{
	private readonly INodeSerializer<T> _elementSerializer;

	public NodeArraySerializer(INodeSerializer<T> elementSerializer) { _elementSerializer = elementSerializer; }

	public int? NodeSize => null;

	public int NodeSizeForObject(T[] array) => array.Length * sizeof(ulong);

	public void Serialize(T[] array, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		for (int i = 0; i < array.Length; i++)
		{
			var hash = _elementSerializer.SerializeToHash(array[i], dataSink);
			ByteEncoder.CopyBytes(hash, writeBuffer.Slice(i * sizeof(ulong), sizeof(ulong)));
		}
	}

	public T[] Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		var elementCount = readBuffer.Length / sizeof(ulong);

		var result = new T[elementCount];
		for (int i = 0; i < elementCount; i++)
		{
			var hash = ByteEncoder.GetUInt64(readBuffer.Slice(i * sizeof(ulong), sizeof(ulong)));
			result[i] = _elementSerializer.DeserializeFromHash(hash, dataSource);
		}

		return result;
	}
}
