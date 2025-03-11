using System;
using System.Collections.Generic;
using Pando.Vaults;

namespace Pando.Serialization.Collections;

/// <summary>
/// Serializer for <see cref="List{T}"/>.
/// </summary>
public class ListSerializer<TElement>(IPandoSerializer<TElement> elementSerializer)
	: CollectionSerializer<List<TElement>, TElement>(elementSerializer)
{
	protected override List<TElement> CreateCollection(
		ReadOnlySpan<byte> elementBytes,
		int elementSize,
		IReadOnlyNodeVault nodeVault
	)
	{
		var list = new List<TElement>(elementBytes.Length / elementSize);
		for (int i = 0; i < elementBytes.Length; i += elementSize)
		{
			var element = ElementSerializer.Deserialize(elementBytes.Slice(i, elementSize), nodeVault);
			list.Add(element);
		}

		return list;
	}
}
