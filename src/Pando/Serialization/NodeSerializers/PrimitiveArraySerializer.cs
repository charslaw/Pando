using System;
using System.Buffers;
using Pando.DataSources;
using Pando.Serialization.PrimitiveSerializers;

namespace Pando.Serialization.NodeSerializers;

/// Serializes a node that is an array of primitive data types using the given primitive serializer.
public class PrimitiveArraySerializer<T> : INodeSerializer<T[]>
{
	private readonly IPrimitiveSerializer<T> _elementSerializer;

	public PrimitiveArraySerializer(IPrimitiveSerializer<T> elementSerializer) { _elementSerializer = elementSerializer; }

	public int? NodeSize => null;

	public int NodeSizeForObject(T[] array)
	{
		var elementByteCount = _elementSerializer.ByteCount;
		if (elementByteCount is not null) return elementByteCount.Value * array.Length;

		// if we get here, elementSerializer is variable size
		var size = 0;
		foreach (var element in array)
		{
			size += _elementSerializer.ByteCountForValue(element);
		}

		return size;
	}

	public void Serialize(T[] array, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		foreach (var element in array)
		{
			_elementSerializer.Serialize(element, ref writeBuffer);
		}
	}

	public T[] Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		var size = readBuffer.Length / _elementSerializer.ByteCount;

		if (size is not null)
		{
			var array = new T[size.Value];

			for (int i = 0; i < size; i++)
			{
				array[i] = _elementSerializer.Deserialize(ref readBuffer);
			}

			return array;
		}
		else
		{
			// We don't know how many elements are contained in `bytes`,
			// so do it dynamically by renting successively larger arrays from the array pool until we've exhausted the bytes array.
			var pool = ArrayPool<T>.Shared;
			var tmpArray = pool.Rent(4);
			var count = 0;
			while (readBuffer.Length > 0)
			{
				if (count + 1 > tmpArray.Length)
				{
					var newArray = pool.Rent(count + 1);
					Array.Copy(tmpArray, newArray, count);
					pool.Return(tmpArray);
					tmpArray = newArray;
				}

				tmpArray[count] = _elementSerializer.Deserialize(ref readBuffer);
				count++;
			}

			T[] result = new T[count];
			Array.Copy(tmpArray, result, count);
			pool.Return(tmpArray);
			return result;
		}
	}
}
