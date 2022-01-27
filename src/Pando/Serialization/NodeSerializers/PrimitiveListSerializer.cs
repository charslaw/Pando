using System;
using System.Collections.Generic;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers.EnumerableFactory;
using Pando.Serialization.PrimitiveSerializers;

namespace Pando.Serialization.NodeSerializers;

/// <summary>
/// Serializes and deserializes an IList of primitive values using the given element serializer.
/// Creation of the specific concrete list type is facilitated by a given <see cref="IEnumerableFactory{TEnumerable,T}"/>
/// </summary>
/// <remarks>
/// This is a subclass of <see cref="PrimitiveEnumerableSerializer{TEnumerable,T}"/> that contains optimizations for
/// enumerables that implement <see cref="IList{T}"/>. This implementation avoids boxing and enumeration in cases where it is not
/// necessary for list-like types, such as getting the length of the list or iterating over it (avoids boxing enumerable and
/// allocating enumerator).
/// </remarks>
/// <typeparam name="TList">The concrete type of the list that will be serialized by this serializer.</typeparam>
/// <typeparam name="T">The type of the elements in the collection serialized by this serializer.</typeparam>
public class PrimitiveListSerializer<TList, T> : PrimitiveEnumerableSerializer<TList, T>
	where TList : IList<T>
{
	public PrimitiveListSerializer(IPrimitiveSerializer<T> elementSerializer, IEnumerableFactory<TList, T> listFactory)
		: base(elementSerializer, listFactory) { }

	public override int NodeSizeForObject(TList list)
	{
		var elementCount = list.Count;
		var perElementByteCount = ElementSerializer.ByteCount;
		if (perElementByteCount is not null)
		{
			return perElementByteCount.Value * elementCount;
		}

		// if we get here, elementSerializer is variable size
		var totalSize = 0;
		for (int i = 0; i < elementCount; i++)
		{
			var element = list[i];
			totalSize += ElementSerializer.ByteCountForValue(element);
		}

		return totalSize;
	}

	public override void Serialize(TList list, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		var elementCount = list.Count;
		for (int i = 0; i < elementCount; i++)
		{
			var element = list[i];
			ElementSerializer.Serialize(element, ref writeBuffer);
		}
	}
}
