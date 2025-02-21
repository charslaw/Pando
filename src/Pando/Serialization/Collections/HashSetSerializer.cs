using System;
using System.Collections.Generic;
using Pando.DataSources;

namespace Pando.Serialization.Collections;

/// <summary>
/// Serializer for <see cref="HashSet{T}"/>
/// </summary>
public class HashSetSerializer<TElement>(IPandoSerializer<TElement> elementSerializer)
	: CollectionSerializer<HashSet<TElement>, TElement>(elementSerializer)
{
	protected override HashSet<TElement> CreateCollection(ReadOnlySpan<byte> elementBytes, int elementSize, IReadOnlyNodeDataStore dataSource)
	{
		var set = new HashSet<TElement>(elementBytes.Length / elementSize);
		for (int i = 0; i < elementBytes.Length; i += elementSize)
		{
			var element = ElementSerializer.Deserialize(elementBytes.Slice(i, elementSize), dataSource);
			set.Add(element);
		}

		return set;
	}
}
