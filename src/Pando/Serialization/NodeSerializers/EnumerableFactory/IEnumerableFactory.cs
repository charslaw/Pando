using System;

namespace Pando.Serialization.NodeSerializers.EnumerableFactory;

public interface IEnumerableFactory<out TEnumerable, T>
{
	public TEnumerable Create(ReadOnlySpan<T> elements);
}
