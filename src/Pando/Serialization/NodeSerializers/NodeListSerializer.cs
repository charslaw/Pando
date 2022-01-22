using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.NodeSerializers;

public class NodeListSerializer<T> : BaseNodeListSerializer<List<T>, T, NodeListSerializer<T>.ListBuilder>
{
	public NodeListSerializer(INodeSerializer<T> elementSerializer) : base(elementSerializer) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override int ListCount(List<T> list) => list.Count;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override T ListGetElement(List<T> list, int index) => list[index];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override ListBuilder CreateListBuilder(int size) => new(size);

	public readonly struct ListBuilder : INodeListBuilder
	{
		private readonly List<T> _list;

		public ListBuilder(int size)
		{
			_list = new List<T>(size);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T value) => _list.Add(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public List<T> Build() => _list;
	}
}
