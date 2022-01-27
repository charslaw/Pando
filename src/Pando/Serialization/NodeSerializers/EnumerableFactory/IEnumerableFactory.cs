using System;

namespace Pando.Serialization.NodeSerializers.EnumerableFactory;

/// An object that can create the specified type of enumerable from a span of elements.
public interface IEnumerableFactory<out TEnumerable, T>
{
	public TEnumerable Create(ReadOnlySpan<T> elements);
}
