using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Serialization.NodeSerializers.EnumerableFactory;

namespace Pando.Serialization.NodeSerializers;

/// <summary>
/// Serializes and deserializes an enumerable type of state tree nodes using the given element serializer.
/// </summary>
/// <remarks>
/// This class can incur some boxing allocation overhead for struct enumerables, and will allocate when getting an enumerator for the enumerable.
/// Consider using <see cref="NodeListSerializer{TList,T}"/> for collection types that can be int indexed.
/// </remarks>
/// <typeparam name="TEnumerable">The concrete type of the enumerable that will be serialized by this serializer.</typeparam>
/// <typeparam name="T">The type of the elements in the collection serialized by this serializer.</typeparam>
public class NodeEnumerableSerializer<TEnumerable, T> : INodeSerializer<TEnumerable>
	where TEnumerable : IEnumerable<T>
{
	protected readonly INodeSerializer<T> ElementSerializer;
	protected readonly IEnumerableFactory<TEnumerable, T> EnumerableFactory;

	public NodeEnumerableSerializer(INodeSerializer<T> elementSerializer, IEnumerableFactory<TEnumerable, T> enumerableFactory)
	{
		ElementSerializer = elementSerializer;
		EnumerableFactory = enumerableFactory;
	}

	public int? NodeSize => null;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual int NodeSizeForObject(TEnumerable enumerable) => enumerable.Count();

	public virtual void Serialize(TEnumerable enumerable, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		foreach (var element in enumerable)
		{
			var hash = ElementSerializer.SerializeToHash(element, dataSink);
			ByteEncoder.CopyBytes(hash, writeBuffer);
			writeBuffer = writeBuffer[sizeof(ulong)..];
		}
	}

	public TEnumerable Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		var elementCount = readBuffer.Length / sizeof(ulong);

		var items = ArrayPool<T>.Shared.Rent(elementCount);
		for (int i = 0; i < elementCount; i++)
		{
			var hash = ByteEncoder.GetUInt64(readBuffer.Slice(i * sizeof(ulong), sizeof(ulong)));
			items[i] = ElementSerializer.DeserializeFromHash(hash, dataSource);
		}

		var result = EnumerableFactory.Create(items.AsSpan(0, elementCount));
		ArrayPool<T>.Shared.Return(items);
		return result;
	}
}
