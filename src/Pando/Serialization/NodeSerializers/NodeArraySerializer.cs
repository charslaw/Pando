using System.Runtime.CompilerServices;

namespace Pando.Serialization.NodeSerializers;

/// Serializes a node that is an array of nodes using the given node serializer.
public class NodeArraySerializer<T> : BaseNodeListSerializer<T[], T, NodeArraySerializer<T>.ArrayNodeListBuilder>
{
	public NodeArraySerializer(INodeSerializer<T> elementSerializer) : base(elementSerializer) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override int ListCount(T[] array) => array.Length;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override T ListGetElement(T[] array, int index) => array[index];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override ArrayNodeListBuilder CreateListBuilder(int size) => new(size);

	/// INodeListBuilder implementation for a basic array.
	/// The _array instance here is intended to live beyond the lifetime of the ArrayNodeListBuilder.
	/// Reusing an ArrayNodeListBuilder is not safe because the internal _array is probably being used elsewhere.
	public struct ArrayNodeListBuilder : INodeListBuilder
	{
		private readonly T[] _array;
		private int _count;

		public ArrayNodeListBuilder(int capacity)
		{
			_count = 0;
			_array = new T[capacity];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T value) => _array[_count++] = value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T[] Build() => _array;

		public void Dispose() { }
	}
}
