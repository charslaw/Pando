using System;
using Pando.DataSources;

namespace Pando.Serialization;

public static class NodeSerializerExtensions
{
	/// Uses this serializer to serialize the given object and write the resulting binary representation to the given data sink.
	/// <returns>The hash of the serialized object in the data sink.</returns>
	public static ulong SerializeToHash<T>(this INodeSerializer<T> serializer, T obj, INodeDataSink dataSink)
	{
		var nodeSize = serializer.NodeSize ?? serializer.NodeSizeForObject(obj);
		Span<byte> buffer = stackalloc byte[nodeSize];
		serializer.Serialize(obj, buffer, dataSink);
		return dataSink.AddNode(buffer);
	}

	/// Uses this serializer to deserialize the object identified by the given hash in the given data source.
	public static T DeserializeFromHash<T>(this INodeSerializer<T> serializer, ulong hash, INodeDataSource dataSource)
	{
		var nodeSize = serializer.NodeSize ?? dataSource.GetSizeOfNode(hash);
		Span<byte> buffer = stackalloc byte[nodeSize];
		dataSource.CopyNodeBytesTo(hash, ref buffer);
		return serializer.Deserialize(buffer, dataSource);
	}
}
