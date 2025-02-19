using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using Pando.DataSources;
using Pando.Serialization.Utils;

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
		var elementBytesArr = ArrayPool<byte>.Shared.Rent(elementBytesSize);

		try
		{
			Span<byte> elementBytes = elementBytesArr.AsSpan(0, elementBytesSize);

			var currentByte = 0;
			foreach (var el in collection)
			{
				ElementSerializer.Serialize(el, elementBytes.Slice(currentByte, elementSize), dataSink);
				currentByte += elementSize;
			}

			var nodeHash = dataSink.AddNode(elementBytes);
			BinaryPrimitives.WriteUInt64LittleEndian(buffer, nodeHash);
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(elementBytesArr);
		}
	}

	public TCollection Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var nodeDataSize = dataSource.GetSizeOfNode(buffer);
		var elementBytesArr = ArrayPool<byte>.Shared.Rent(nodeDataSize);

		try
		{
			Span<byte> elementBytes = elementBytesArr.AsSpan(0, nodeDataSize);
			dataSource.CopyNodeBytesTo(buffer, elementBytes);

			return CreateCollection(elementBytes, ElementSerializer.SerializedSize, dataSource);
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(elementBytesArr);
		}
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, IDataSource dataSource)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		var baseBytesSize = dataSource.GetSizeOfNode(baseBuffer);
		var targetBytesSize = dataSource.GetSizeOfNode(targetBuffer);
		var sourceBytesSize = dataSource.GetSizeOfNode(sourceBuffer);
		var mergedBytesSize = Math.Max(targetBytesSize, sourceBytesSize);
		var sharedBytesSize = Math.Max(mergedBytesSize, baseBytesSize); // the size of the buffer shared by the base data and the merged result
		var totalBytesSize = sharedBytesSize + targetBytesSize + sourceBytesSize;
		var elementSize = ElementSerializer.SerializedSize;

		// allocate a buffer to contain the element data of base, target, and source
		var totalBytesArr = ArrayPool<byte>.Shared.Rent(totalBytesSize);

		try
		{
			Span<byte> totalBytesBuffer = totalBytesArr.AsSpan(0, totalBytesSize);

			var sharedBytesBuffer = totalBytesBuffer[..sharedBytesSize];
			dataSource.CopyNodeBytesTo(baseBuffer, sharedBytesBuffer[..baseBytesSize]);
			var targetBytesBuffer = totalBytesBuffer.Slice(sharedBytesSize, targetBytesSize);
			dataSource.CopyNodeBytesTo(targetBuffer, targetBytesBuffer);
			var sourceBytesBuffer = totalBytesBuffer.Slice(sharedBytesSize + targetBytesSize, sourceBytesSize);
			dataSource.CopyNodeBytesTo(sourceBuffer, sourceBytesBuffer);

			for (int i = 0; i < mergedBytesSize; i += elementSize)
			{
				if (i >= targetBytesSize)
				{
					// we've gone off the end of the target, copy the rest of source into base children buffer
					sourceBytesBuffer[i..].CopyTo(sharedBytesBuffer[i..mergedBytesSize]);
					break;
				}

				if (i >= sourceBytesSize)
				{
					// we've gone off the end of the source, copy the rest of target into base children buffer
					targetBytesBuffer[i..].CopyTo(sharedBytesBuffer[i..mergedBytesSize]);
					break;
				}

				// merge element
				ElementSerializer.Merge(
					sharedBytesBuffer.Slice(i, elementSize),
					targetBytesBuffer.Slice(i, elementSize),
					sourceBytesBuffer.Slice(i, elementSize),
					dataSource
				);
			}

			dataSource.AddNode(sharedBytesBuffer[..mergedBytesSize], baseBuffer);
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(totalBytesArr);
		}
	}

	protected abstract TCollection CreateCollection(ReadOnlySpan<byte> elementBytes, int elementSize, INodeDataSource dataSource);
}
