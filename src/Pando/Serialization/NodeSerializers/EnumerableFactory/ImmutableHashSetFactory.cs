using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Pando.Serialization.NodeSerializers.EnumerableFactory;

public class ImmutableHashSetFactory<T> : IEnumerableFactory<ImmutableHashSet<T>, T>
{
	private readonly IEqualityComparer<T> _equalityComparer;

	public ImmutableHashSetFactory() : this(EqualityComparer<T>.Default) { }

	public ImmutableHashSetFactory(IEqualityComparer<T> equalityComparer)
	{
		_equalityComparer = equalityComparer;
	}

	public ImmutableHashSet<T> Create(ReadOnlySpan<T> elements)
	{
		var builder = ImmutableHashSet.CreateBuilder(_equalityComparer);
		for (int i = 0; i < elements.Length; i++)
		{
			builder.Add(elements[i]);
		}

		return builder.ToImmutableHashSet();
	}
}
