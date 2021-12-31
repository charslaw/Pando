using System;
using Pando.DataSources;

namespace Pando.Serialization;

public static class NodeSerializerExtensions
{
	public static T DeserializeFromHash<T>(this INodeSerializer<T> serializer, ulong hash, INodeDataSource dataSource)
	{
		var nodeSize = serializer.NodeSize ?? dataSource.GetSizeOfNode(hash);
		Span<byte> buffer = stackalloc byte[nodeSize];
		dataSource.CopyNodeBytesTo(hash, ref buffer);
		return serializer.Deserialize(buffer, dataSource);
	}
}
