using System;
using System.Buffers;
using Pando.DataSources;
using Pando.Serialization.PrimitiveSerializers;

namespace Pando.Serialization;

/// Serializes a node that is an array of primitive data types using the given primitive serializer.
public class PrimitiveArrayNodeSerializer<T> : INodeSerializer<T[]>
{
	public int? NodeSize => null;

	private readonly IPrimitiveSerializer<T> _elementSerializer;

	public PrimitiveArrayNodeSerializer(IPrimitiveSerializer<T> elementSerializer) { _elementSerializer = elementSerializer; }

	public ulong Serialize(T[] array, INodeDataSink dataSink)
	{
		int size = 0;

		if (_elementSerializer.ByteCount is not null)
		{
			size = _elementSerializer.ByteCount.Value * array.Length;
		}
		else
		{
			foreach (var element in array)
			{
				size += _elementSerializer.ByteCountForValue(element);
			}
		}

		Span<byte> buffer = stackalloc byte[size];

		var writeBuffer = buffer;

		foreach (var element in array)
		{
			_elementSerializer.Serialize(element, ref writeBuffer);
		}

		return dataSink.AddNode(buffer);
	}

	public T[] Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource)
	{
		var size = bytes.Length / _elementSerializer.ByteCount;

		if (size is not null)
		{
			var array = new T[size.Value];

			for (int i = 0; i < size; i++)
			{
				array[i] = _elementSerializer.Deserialize(ref bytes);
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
			while (bytes.Length > 0)
			{
				if (count + 1 > tmpArray.Length)
				{
					var newArray = pool.Rent(count + 1);
					Array.Copy(tmpArray, newArray, count);
					pool.Return(tmpArray);
					tmpArray = newArray;
				}

				tmpArray[count] = _elementSerializer.Deserialize(ref bytes);
				count++;
			}

			T[] result = new T[count];
			Array.Copy(tmpArray, result, count);
			pool.Return(tmpArray);
			return result;
		}
	}
}
