using System;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.NodeSerializers;

/// Serializes a node that is an array of nodes using the given node serializer.
public class NodeArraySerializer<T> : BaseNodeListSerializer<T[], T>
{
	public NodeArraySerializer(INodeSerializer<T> elementSerializer) : base(elementSerializer) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override int ListCount(T[] array) => array.Length;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override T ListGetElement(T[] array, int index) => array[index];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override T[] CreateList(ReadOnlySpan<T> items) => items.ToArray();
}
