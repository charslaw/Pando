using System;
using System.Collections.Generic;

namespace Pando.Serialization.NodeSerializers.EnumerableFactory;

public class ListFactory<T> : IEnumerableFactory<List<T>, T>
{
	public List<T> Create(ReadOnlySpan<T> elements)
	{
		var list = new List<T>(elements.Length);
		for (int i = 0; i < elements.Length; i++)
		{
			list.Add(elements[i]);
		}

		return list;
	}
}
