using System;
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
/// <typeparam name="TListBuilder">The type of the INodeListBuilder used to create a new instance of the list when deserializing.
/// This is a generic parameter so that we can avoid boxing struct implementations of INodeListBuilder.</typeparam>
public abstract class BaseNodeListSerializer<TList, T, TListBuilder> : INodeSerializer<TList>
	where TListBuilder : BaseNodeListSerializer<TList, T, TListBuilder>.INodeListBuilder
{
	private readonly INodeSerializer<T> _elementSerializer;

	internal BaseNodeListSerializer(INodeSerializer<T> elementSerializer) { _elementSerializer = elementSerializer; }

	/// When overridden in a derived class, returns the number of elements in the given list.
	protected abstract int ListCount(TList list);

	/// When overridden in a derived class, gets the element at the given index from the given list.
	protected abstract T ListGetElement(TList list, int index);

	/// When overridden in a derived class, returns a single-use instance of the INodeListBuilder designated for this class.
	/// <param name="size">The size of the final list. The list is guaranteed to be exactly this length.</param>
	protected abstract TListBuilder CreateListBuilder(int size);

	/// A builder object for lists of type TList that contain elements of type T.
	/// Used to generically construct an instance of the resulting list type when deserializing.
	/// Implements IDisposable because the INodeListBuilder might have resources it needs to dispose of,
	/// e.g. returning an array rented from ArrayPool, etc.
	/// INodeListBuilders are intended to be single-use (i.e. they're created, used, then disposed).
	public interface INodeListBuilder : IDisposable
	{
		public void Add(T value);
		public TList Build();
	}

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

		using var result = CreateListBuilder(elementCount);
		for (int i = 0; i < elementCount; i++)
		{
			var hash = ByteEncoder.GetUInt64(readBuffer.Slice(i * sizeof(ulong), sizeof(ulong)));
			result.Add(_elementSerializer.DeserializeFromHash(hash, dataSource));
		}

		return result.Build();
	}
}
