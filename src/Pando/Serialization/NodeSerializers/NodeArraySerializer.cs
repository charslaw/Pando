using System.Runtime.CompilerServices;

namespace Pando.Serialization.NodeSerializers;

/// Serializes a node that is an array of nodes using the given node serializer.
public class NodeArraySerializer<T> : BaseNodeListSerializer<T[], T, NodeArraySerializer<T>.ArrayBuilder>
{
	public NodeArraySerializer(INodeSerializer<T> elementSerializer) : base(elementSerializer) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override int ListCount(T[] array) => array.Length;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override T ListGetElement(T[] array, int index) => array[index];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override ArrayBuilder CreateListBuilder(int size) => new(size);

	// struct to avoid allocations beyond the actual array instance
	public struct ArrayBuilder : INodeListBuilder
	{
		private readonly T[] _array;
		private int _count;

		public ArrayBuilder(int capacity)
		{
			_count = 0;
			_array = new T[capacity];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T value) => _array[_count++] = value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T[] Build() => _array;
	}
}
