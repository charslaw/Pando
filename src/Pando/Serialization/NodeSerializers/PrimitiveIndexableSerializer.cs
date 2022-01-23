using System;
using System.Buffers;
using Pando.DataSources;
using Pando.Serialization.PrimitiveSerializers;

namespace Pando.Serialization.NodeSerializers;

/// <summary>
/// Serializes and deserializes a generic indexable collection of primitive values using the given element serializer.
/// Access to and creation of the specific concrete indexable collection is facilitated by a given <see cref="IIndexableAdapter{TIndexable, T}"/>
/// </summary>
/// <typeparam name="TIndexable">The concrete type of the indexable collection that will be serialized by this serializer.</typeparam>
/// <typeparam name="T">The type of the elements in the collection serialized by this serializer.</typeparam>
public class PrimitiveIndexableSerializer<TIndexable, T> : INodeSerializer<TIndexable>
{
	private readonly IPrimitiveSerializer<T> _elementSerializer;
	private readonly IIndexableAdapter<TIndexable, T> _indexableAdapter;

	public PrimitiveIndexableSerializer(IPrimitiveSerializer<T> elementSerializer, IIndexableAdapter<TIndexable, T> indexableAdapter)
	{
		_elementSerializer = elementSerializer;
		_indexableAdapter = indexableAdapter;
	}

	public int? NodeSize => null;

	public int NodeSizeForObject(TIndexable indexable)
	{
		var elementCount = _indexableAdapter.Count(indexable);
		var perElementByteCount = _elementSerializer.ByteCount;
		if (perElementByteCount is not null) return perElementByteCount.Value * elementCount;

		// if we get here, elementSerializer is variable size
		var size = 0;
		for (int i = 0; i < elementCount; i++)
		{
			var element = _indexableAdapter.Get(indexable, i);
			size += _elementSerializer.ByteCountForValue(element);
		}

		return size;
	}

	public void Serialize(TIndexable indexable, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		var listCount = _indexableAdapter.Count(indexable);
		for (int i = 0; i < listCount; i++)
		{
			var element = _indexableAdapter.Get(indexable, i);
			_elementSerializer.Serialize(element, ref writeBuffer);
		}
	}

	public TIndexable Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		var elementCount = readBuffer.Length / _elementSerializer.ByteCount;

		if (elementCount is not null)
		{
			var elements = ArrayPool<T>.Shared.Rent(elementCount.Value);

			for (int i = 0; i < elementCount; i++)
			{
				elements[i] = _elementSerializer.Deserialize(ref readBuffer);
			}

			var result = _indexableAdapter.Create(elements.AsSpan(0, elementCount.Value));
			ArrayPool<T>.Shared.Return(elements);
			return result;
		}
		else
		{
			// We don't know how many elements are contained in `bytes`, so create it dynamically by
			// renting successively larger arrays from the array pool until we've exhausted the bytes array.
			var pool = ArrayPool<T>.Shared;
			var elements = pool.Rent(4);
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

				elements[count] = _elementSerializer.Deserialize(ref readBuffer);
				count++;
			}

			var result = _indexableAdapter.Create(elements.AsSpan(0, count));
			pool.Return(elements);
			return result;
		}
	}
}
