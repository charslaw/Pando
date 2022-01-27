using System;
using System.Collections.Generic;

namespace Pando.Serialization.NodeSerializers.EnumerableFactory;

public class HashSetFactory<T> : IEnumerableFactory<HashSet<T>, T>
{
	private readonly IEqualityComparer<T> _equalityComparer;

	public HashSetFactory() : this(EqualityComparer<T>.Default) { }

	public HashSetFactory(IEqualityComparer<T> equalityComparer)
	{
		_equalityComparer = equalityComparer;
	}

	public HashSet<T> Create(ReadOnlySpan<T> elements)
	{
		var set = new HashSet<T>(elements.Length, _equalityComparer);
		for (int i = 0; i < elements.Length; i++)
		{
			set.Add(elements[i]);
		}

		return set;
	}
}
