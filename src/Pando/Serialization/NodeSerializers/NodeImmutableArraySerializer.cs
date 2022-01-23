using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.NodeSerializers;

/// Serializes a node that is an immutable array of nodes using the given node serializer.
public class NodeImmutableArraySerializer<T> : BaseNodeListSerializer<ImmutableArray<T>, T>
{
	private readonly ImmutableArray<T>.Builder _builder = ImmutableArray.CreateBuilder<T>();

	public NodeImmutableArraySerializer(INodeSerializer<T> elementSerializer) : base(elementSerializer) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override int ListCount(ImmutableArray<T> array) => array.Length;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override T ListGetElement(ImmutableArray<T> array, int index) => array[index];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override ImmutableArray<T> CreateList(ReadOnlySpan<T> items)
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
