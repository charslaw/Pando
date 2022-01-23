using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Pando.DataSources;
using Pando.DataSources.Utils;

namespace Pando.Serialization.NodeSerializers;

/// <summary>
/// <p>A base class for node serializers that serialize list-ish collection of nodes (index-able ordered collections).
/// The verbiage of this documentation and code uses "list" to refer to this collection, but it does not necessarily need
/// to be a <see cref="System.Collections.Generic.List{T}"/></p>
///
/// <p>This class handles all of the plumbing of getting the sub-elements out of the list and serializing them,
/// or the reverse for deserializing them.</p>
///
/// <p>Derived classes must implement abstract methods that allow the base serializer to get the number of elements in the
/// list, get elements at a specific index out of the list, and provide an implementation of an interface
/// that allows for generically creating the list type with a list-like interface.</p>
/// </summary>
/// <typeparam name="TList">The type of the list that will be serialized.</typeparam>
/// <typeparam name="T">The type of the elements in the list.</typeparam>
public abstract class BaseNodeListSerializer<TList, T> : INodeSerializer<TList>
{
	private readonly INodeSerializer<T> _elementSerializer;

	protected BaseNodeListSerializer(INodeSerializer<T> elementSerializer) { _elementSerializer = elementSerializer; }

	/// When overridden in a derived class, returns the number of elements in the given list.
	protected abstract int ListCount(TList list);

	/// When overridden in a derived class, gets the element at the given index from the given list.
	protected abstract T ListGetElement(TList list, int index);

	/// When overridden in a derived class, converts a span of items into the appropriate type for this serializer.
	protected abstract TList CreateList(ReadOnlySpan<T> items);

	public int? NodeSize => null;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int NodeSizeForObject(TList nodeList) => ListCount(nodeList) * sizeof(ulong);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(TList nodeList, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		for (int i = 0; i < ListCount(nodeList); i++)
		{
			var hash = _elementSerializer.SerializeToHash(ListGetElement(nodeList, i), dataSink);
			ByteEncoder.CopyBytes(hash, writeBuffer.Slice(i * sizeof(ulong), sizeof(ulong)));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TList Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		var elementCount = readBuffer.Length / sizeof(ulong);

		var items = ArrayPool<T>.Shared.Rent(elementCount);
		for (int i = 0; i < elementCount; i++)
		{
			var hash = ByteEncoder.GetUInt64(readBuffer.Slice(i * sizeof(ulong), sizeof(ulong)));
			items[i] = _elementSerializer.DeserializeFromHash(hash, dataSource);
		}

		var result = CreateList(items.AsSpan(0, elementCount));
		ArrayPool<T>.Shared.Return(items);
		return result;
	}
}
