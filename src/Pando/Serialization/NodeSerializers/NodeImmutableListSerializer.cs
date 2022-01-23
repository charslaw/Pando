using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.NodeSerializers;

public class NodeImmutableListSerializer<T> : BaseNodeListSerializer<ImmutableList<T>, T>
{
	private readonly ImmutableList<T>.Builder _builder = ImmutableList.CreateBuilder<T>();

	public NodeImmutableListSerializer(INodeSerializer<T> elementSerializer) : base(elementSerializer) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override int ListCount(ImmutableList<T> list) => list.Count;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override T ListGetElement(ImmutableList<T> list, int index) => list[index];

	protected override ImmutableList<T> CreateList(ReadOnlySpan<T> items)
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
