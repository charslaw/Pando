using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.NodeSerializers.EnumerableFactory;

public class ImmutableArrayFactory<T> : IEnumerableFactory<ImmutableArray<T>, T>
{
	public ImmutableArray<T> Create(ReadOnlySpan<T> elements)
	{
		var array = new T[elements.Length];
		elements.CopyTo(array);
		return Unsafe.As<T[], ImmutableArray<T>>(ref array);
	}
}
