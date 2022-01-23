using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Pando.DataSources;
using Pando.DataSources.Utils;

namespace Pando.Serialization.NodeSerializers;

/// <summary>
/// Serializes and deserializes a generic indexable collection of state tree nodes using the given element serializer.
/// Access to and creation of the specific concrete indexable collection is facilitated by a given <see cref="IIndexableAdapter{TIndexable, T}"/>
/// </summary>
/// <typeparam name="TIndexable">The concrete type of the indexable collection that will be serialized by this serializer.</typeparam>
/// <typeparam name="T">The type of the elements in the collection serialized by this serializer.</typeparam>
public class NodeIndexableSerializer<TIndexable, T> : INodeSerializer<TIndexable>
{
	private readonly INodeSerializer<T> _elementSerializer;
	private readonly IIndexableAdapter<TIndexable, T> _indexableAdapter;

	public NodeIndexableSerializer(INodeSerializer<T> elementSerializer, IIndexableAdapter<TIndexable, T> indexableAdapter)
	{
		_elementSerializer = elementSerializer;
		_indexableAdapter = indexableAdapter;
	}

	public int? NodeSize => null;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int NodeSizeForObject(TIndexable nodeList) => _indexableAdapter.Count(nodeList) * sizeof(ulong);

	public void Serialize(TIndexable nodeList, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		for (int i = 0; i < _indexableAdapter.Count(nodeList); i++)
		{
			var hash = _elementSerializer.SerializeToHash(_indexableAdapter.Get(nodeList, i), dataSink);
			ByteEncoder.CopyBytes(hash, writeBuffer.Slice(i * sizeof(ulong), sizeof(ulong)));
		}
	}

	public TIndexable Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		var elementCount = readBuffer.Length / sizeof(ulong);

		var items = ArrayPool<T>.Shared.Rent(elementCount);
		for (int i = 0; i < elementCount; i++)
		{
			var hash = ByteEncoder.GetUInt64(readBuffer.Slice(i * sizeof(ulong), sizeof(ulong)));
			items[i] = _elementSerializer.DeserializeFromHash(hash, dataSource);
		}

		var result = _indexableAdapter.Create(items.AsSpan(0, elementCount));
		ArrayPool<T>.Shared.Return(items);
		return result;
	}
}
