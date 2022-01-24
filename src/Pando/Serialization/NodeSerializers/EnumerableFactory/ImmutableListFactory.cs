using System;
using System.Collections.Immutable;

namespace Pando.Serialization.NodeSerializers.EnumerableFactory;

public class ImmutableListFactory<T> : IEnumerableFactory<ImmutableList<T>, T>
{
	private readonly ImmutableList<T>.Builder _builder = ImmutableList.CreateBuilder<T>();

	public ImmutableList<T> Create(ReadOnlySpan<T> elements)
	{
		lock (_builder)
		{
			for (int i = 0; i < elements.Length; i++)
			{
				_builder[i] = elements[i];
			}

			var result = _builder.ToImmutable();
			_builder.Clear();
			return result;
		}
	}
}
