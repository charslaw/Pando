using System;
using System.Collections.Immutable;

namespace Pando.Serialization.NodeSerializers;

public class ImmutableListAdapter<T> : IIndexableAdapter<ImmutableList<T>, T>
{
	private readonly ImmutableList<T>.Builder _builder = ImmutableList.CreateBuilder<T>();

	public int Count(ImmutableList<T> list) => list.Count;
	public T Get(ImmutableList<T> list, int index) => list[index];

	public ImmutableList<T> Create(ReadOnlySpan<T> items)
	{
		lock (_builder)
		{
			for (int i = 0; i < items.Length; i++)
			{
				_builder[i] = items[i];
			}

			var result = _builder.ToImmutable();
			_builder.Clear();
			return result;
		}
	}
}
