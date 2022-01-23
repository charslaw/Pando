using System;
using System.Collections.Immutable;

namespace Pando.Serialization.NodeSerializers;

/// Defines methods for accessing and creating an immutable array
public class ImmutableArrayAdapter<T> : IIndexableAdapter<ImmutableArray<T>, T>
{
	private readonly ImmutableArray<T>.Builder _builder = ImmutableArray.CreateBuilder<T>();

	public int Count(ImmutableArray<T> array) => array.Length;
	public T Get(ImmutableArray<T> array, int index) => array[index];

	public ImmutableArray<T> Create(ReadOnlySpan<T> items)
	{
		lock (_builder)
		{
			_builder.Count = items.Length;
			for (int i = 0; i < items.Length; i++)
			{
				_builder[i] = items[i];
			}

			return _builder.MoveToImmutable();
		}
	}
}
