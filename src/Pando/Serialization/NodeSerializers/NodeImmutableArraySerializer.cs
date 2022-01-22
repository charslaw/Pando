using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.NodeSerializers;

/// Serializes a node that is an immutable array of nodes using the given node serializer.
public class NodeImmutableArraySerializer<T>
	: BaseNodeListSerializer<ImmutableArray<T>, T, NodeImmutableArraySerializer<T>.ImmutableArrayBuilder>
{
	public NodeImmutableArraySerializer(INodeSerializer<T> elementSerializer) : base(elementSerializer) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override int ListCount(ImmutableArray<T> array) => array.Length;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override T ListGetElement(ImmutableArray<T> array, int index) => array[index];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override ImmutableArrayBuilder CreateListBuilder(int size) => new(size);

	// struct to avoid allocations beyond the actual array instance
	// allocating our own array as opposed to using ImmutableArray.Builder avoids the minor GC pressure of creating
	// the "native" ImmutableArray.Builder and then discarding it a short time later.
	public struct ImmutableArrayBuilder : INodeListBuilder
	{
		private T[] _array;
		private int _count;

		public ImmutableArrayBuilder(int size)
		{
			_array = new T[size];
			_count = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T value) => _array[_count++] = value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArray<T> Build() => Unsafe.As<T[], ImmutableArray<T>>(ref _array);

		public void Dispose() { }
	}
}
