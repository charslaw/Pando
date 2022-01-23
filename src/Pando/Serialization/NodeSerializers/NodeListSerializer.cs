using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.NodeSerializers;

public class NodeListSerializer<T> : BaseNodeListSerializer<List<T>, T>
{
	public NodeListSerializer(INodeSerializer<T> elementSerializer) : base(elementSerializer) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override int ListCount(List<T> list) => list.Count;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override T ListGetElement(List<T> list, int index) => list[index];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override List<T> CreateList(ReadOnlySpan<T> items)
	{
		var list = new List<T>(items.Length);
		for (int i = 0; i < items.Length; i++)
		{
			list.Add(items[i]);
		}

		return list;
	}
}
