using System;
using System.Collections.Generic;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Serialization.NodeSerializers.EnumerableFactory;

namespace Pando.Serialization.NodeSerializers;

/// <summary>Serializes and deserializes an <see cref="IList{T}"/> of state tree nodes using the given element serializer.</summary>
/// <remarks>
/// This is a subclass of <see cref="NodeEnumerableSerializer{TEnumerable,T}"/> that contains optimizations for
/// enumerables that implement <see cref="IList{T}"/>. This implementation avoids boxing and enumeration in cases where it is not
/// necessary for list-like types, such as getting the length of the list or iterating over it (avoids boxing enumerable and
/// allocating enumerator).
/// </remarks>
/// <typeparam name="TList">The concrete type of the list that will be serialized by this serializer.</typeparam>
/// <typeparam name="T">The type of the elements in the collection serialized by this serializer.</typeparam>
public class NodeListSerializer<TList, T> : NodeEnumerableSerializer<TList, T>
	where TList : IList<T>
{
	public NodeListSerializer(INodeSerializer<T> elementSerializer, IEnumerableFactory<TList, T> enumerableFactory)
		: base(elementSerializer, enumerableFactory) { }

	public override int NodeSizeForObject(TList list) => list.Count * sizeof(ulong);

	public override void Serialize(TList list, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		for (int i = 0; i < list.Count; i++)
		{
			var hash = ElementSerializer.SerializeToHash(list[i], dataSink);
			ByteEncoder.CopyBytes(hash, writeBuffer.Slice(i * sizeof(ulong), sizeof(ulong)));
		}
	}
}
