using System;
using System.Buffers;

namespace Pando.Serialization.Utils;

public static class ArrayPoolExtensions
{
	/// Stack only utility to automatically handle returning a pooled array to the pool to which it belongs
	public readonly ref struct PooledArrayHandle<T>
	{
		public readonly T[] Array;
		public Span<T> Span { get; }
		public int Length { get; }
		private readonly bool _clearOnReturn;
		private readonly ArrayPool<T> _pool;

		internal PooledArrayHandle(ArrayPool<T> pool, int count, bool clearOnReturn = false)
		{
			_pool = pool;
			_clearOnReturn = clearOnReturn;
			Length = count;
			Array = pool.Rent(count);
			Span = Array.AsSpan(0, count);
		}

		public void Dispose()
		{
			_pool.Return(Array, _clearOnReturn);
		}
	}

	public static PooledArrayHandle<T> RentHandle<T>(this ArrayPool<T> pool, int count, bool clearOnReturn = false)
		=> new(pool, count, clearOnReturn);
}
