using System;

namespace Pando.Serialization.NodeSerializers.EnumerableFactory;

public class ArrayFactory<T> : IEnumerableFactory<T[], T>
{
	public T[] Create(ReadOnlySpan<T> elements) => elements.ToArray();
}
