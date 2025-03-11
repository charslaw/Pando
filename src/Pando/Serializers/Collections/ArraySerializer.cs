using System;
using Pando.Vaults;

namespace Pando.Serializers.Collections;

/// <summary>
/// Serializer for <see cref="Array"/>.
/// </summary>
public class ArraySerializer<TElement>(IPandoSerializer<TElement> elementSerializer)
	: CollectionSerializer<TElement[], TElement>(elementSerializer)
{
	protected override TElement[] CreateCollection(
		ReadOnlySpan<byte> elementBytes,
		int elementSize,
		IReadOnlyNodeVault nodeVault
	)
	{
		var arr = new TElement[elementBytes.Length / elementSize];
		var currentByte = 0;
		for (int i = 0; i < arr.Length; i++)
		{
			var element = ElementSerializer.Deserialize(elementBytes.Slice(currentByte, elementSize), nodeVault);
			arr[i] = element;
			currentByte += elementSize;
		}

		return arr;
	}
}
