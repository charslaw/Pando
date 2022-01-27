using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers.EnumerableFactory;
using Pando.Serialization.PrimitiveSerializers;
using Pando.Serialization.Utils;

namespace Pando.Serialization.NodeSerializers;

/// <summary>
/// Serializes and deserializes a generic indexable collection of primitive values using the given element serializer.
/// Creation of the specific concrete indexable collection is facilitated by a given <see cref="IEnumerableFactory{TEnumerable,T}"/>
/// </summary>
/// <remarks>
/// This class can incur some boxing allocation overhead for struct enumerables, and will allocate when getting an enumerator for the enumerable.
/// Consider using <see cref="PrimitiveListSerializer{TList,T}"/> for collection types that can be int indexed.
/// </remarks>
/// <typeparam name="TEnumerable">The concrete type of the indexable collection that will be serialized by this serializer.</typeparam>
/// <typeparam name="T">The type of the elements in the collection serialized by this serializer.</typeparam>
public class PrimitiveEnumerableSerializer<TEnumerable, T> : INodeSerializer<TEnumerable>
	where TEnumerable : IEnumerable<T>

{
	protected readonly IPrimitiveSerializer<T> ElementSerializer;
	protected readonly IEnumerableFactory<TEnumerable, T> EnumerableFactory;

	public PrimitiveEnumerableSerializer(IPrimitiveSerializer<T> elementSerializer, IEnumerableFactory<TEnumerable, T> enumerableFactory)
	{
		ElementSerializer = elementSerializer;
		EnumerableFactory = enumerableFactory;
	}

	public int? NodeSize => null;

	public virtual int NodeSizeForObject(TEnumerable enumerable)
	{
		var perElementByteCount = ElementSerializer.ByteCount;
		if (perElementByteCount is not null)
		{
			var elementCount = enumerable.Count();
			return perElementByteCount.Value * elementCount;
		}

		// if we get here, elementSerializer is variable size
		var totalSize = 0;
		foreach (var element in enumerable)
		{
			totalSize += ElementSerializer.ByteCountForValue(element);
		}

		return totalSize;
	}

	public virtual void Serialize(TEnumerable enumerable, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		foreach (var element in enumerable)
		{
			ElementSerializer.Serialize(element, ref writeBuffer);
		}
	}

	public TEnumerable Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		var elementCount = readBuffer.Length / ElementSerializer.ByteCount;

		if (elementCount is not null)
		{
			using var arrHandle = ArrayPool<T>.Shared.RentHandle(elementCount.Value);
			var elements = arrHandle.Span;
			for (int i = 0; i < elementCount; i++)
			{
				elements[i] = ElementSerializer.Deserialize(ref readBuffer);
			}

			var result = EnumerableFactory.Create(elements);
			return result;
		}
		else
		{
			// We don't know how many elements are contained in `bytes`, so create it dynamically by
			// renting successively larger arrays from the array pool until we've exhausted the bytes array.
			var pool = ArrayPool<T>.Shared;
			var elements = pool.Rent(4);
			try
			{
				var count = 0;
				while (readBuffer.Length > 0)
				{
					if (count + 1 > elements.Length)
					{
						// This relies on the fact that the shared array pool uses buckets of successive powers of two in size,
						// so requesting 1 higher than the current length should jump to the next bucket, which is 2x the current bucket size.
						var newElements = pool.Rent(elements.Length + 1);
						Array.Copy(elements, newElements, count);
						pool.Return(elements);
						elements = newElements;
					}

					elements[count] = ElementSerializer.Deserialize(ref readBuffer);
					count++;
				}

				var result = EnumerableFactory.Create(elements.AsSpan(0, count));
				return result;
			}
			finally
			{
				pool.Return(elements);
			}
		}
	}
}
