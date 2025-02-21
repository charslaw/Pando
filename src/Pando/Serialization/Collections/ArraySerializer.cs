using System;
using Pando.DataSources;

namespace Pando.Serialization.Collections;

/// <summary>
/// Serializer for <see cref="Array"/>.
/// </summary>
public class ArraySerializer<TElement>(IPandoSerializer<TElement> elementSerializer)
	: CollectionSerializer<TElement[], TElement>(elementSerializer)
{
	protected override TElement[] CreateCollection(ReadOnlySpan<byte> elementBytes, int elementSize, IReadOnlyNodeDataStore dataSource)
	{
		var arr = new TElement[elementBytes.Length / elementSize];
		var currentByte = 0;
		for (int i = 0; i < arr.Length; i++)
		{
			var element = ElementSerializer.Deserialize(elementBytes.Slice(currentByte, elementSize), dataSource);
			arr[i] = element;
			currentByte += elementSize;
		}

		return arr;
	}
}
