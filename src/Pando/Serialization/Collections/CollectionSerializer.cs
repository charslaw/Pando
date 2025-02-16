using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using Pando.DataSources;

namespace Pando.Serialization.Collections;

/// <summary>
/// Base class for serializers of <see cref="ICollection{T}"/>.
/// </summary>
public abstract class CollectionSerializer<TCollection, TElement>(IPandoSerializer<TElement> elementSerializer) : IPandoSerializer<TCollection>
	where TCollection : ICollection<TElement>
{
	protected readonly IPandoSerializer<TElement> ElementSerializer = elementSerializer;

	public int SerializedSize => sizeof(ulong);

	public void Serialize(TCollection collection, Span<byte> buffer, INodeDataSink dataSink)
	{
		var elementSize = ElementSerializer.SerializedSize;
		var elementBytesSize = collection.Count * elementSize;
		Span<byte> elementBytes = stackalloc byte[elementBytesSize];

		var currentByte = 0;
		foreach (var el in collection)
		{
			ElementSerializer.Serialize(el, elementBytes.Slice(currentByte, elementSize), dataSink);
			currentByte += elementSize;
		}

		var nodeHash = dataSink.AddNode(elementBytes);
		BinaryPrimitives.WriteUInt64LittleEndian(buffer, nodeHash);
	}

	public TCollection Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var nodeHash = BinaryPrimitives.ReadUInt64LittleEndian(buffer);
		var nodeDataSize = dataSource.GetSizeOfNode(nodeHash);
		Span<byte> elementBytes = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(nodeHash, elementBytes);

		return CreateCollection(elementBytes, ElementSerializer.SerializedSize, dataSource);
	}

	protected abstract TCollection CreateCollection(ReadOnlySpan<byte> elementBytes, int elementSize, INodeDataSource dataSource);
}
