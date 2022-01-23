using System;
using System.Collections.Generic;

namespace Pando.Serialization.NodeSerializers;

public class ListAdapter<T> : IIndexableAdapter<List<T>, T>
{
	public int Count(List<T> list) => list.Count;

	public T Get(List<T> list, int index) => list[index];

	public List<T> Create(ReadOnlySpan<T> items)
	{
		var list = new List<T>(items.Length);
		for (int i = 0; i < items.Length; i++)
		{
			list.Add(items[i]);
		}

		return list;
	}
}
