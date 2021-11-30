using System;

namespace Pando.Repositories.Utils
{
	/// A collection that you can add a span of data to, and visit a span of.
	/// <remarks><see cref="SpannableList{T}"/> is not thread safe.<br/>
	/// Do not attempt to concurrently add multiple spans to the list.<br/>
	/// Do not attempt to concurrently get from and add to the list.</remarks>
	internal class SpannableList<T>
	{
		private const int EXPANSION_FACTOR = 2;

		public int Count => _head;

		private T[] _array;
		private int _head;

		public SpannableList(int capacity = 8) { _array = new T[capacity]; }

		public SpannableList(ReadOnlySpan<T> initialValues)
		{
			var initialDataLength = initialValues.Length;
			_array = new T[initialDataLength];
			initialValues.CopyTo(_array.AsSpan(0, initialDataLength));
		}

		/// Adds a given span to the spannable list.
		/// <remarks>This method is not reentrant.
		/// Don't call it concurrently with itself or <see cref="SpannableList{T}.VisitSpan{TResult}"/></remarks>
		public DataSlice AddSpan(ReadOnlySpan<T> sourceSpan)
		{
			var sourceLength = sourceSpan.Length;

			EnsureCapacity(sourceLength);

			var start = _head;
			var destSpan = _array.AsSpan(start, sourceLength);
			sourceSpan.CopyTo(destSpan);
			_head += sourceLength;

			return new DataSlice(start, sourceLength);
		}

		private void EnsureCapacity(int length)
		{
			var currentHeadspace = _array.Length - _head;
			if (currentHeadspace >= length) return;

			var minimumNewSize = _head + length;
			Array.Resize(ref _array, minimumNewSize * EXPANSION_FACTOR);
		}

		/// Allows an external consumer to access a span of the list without leaking the span.
		/// <remarks>Don't call this method concurrently with <see cref="SpannableList{T}.AddSpan"/></remarks>
		public TResult VisitSpan<TResult>(int start, int length, SpanVisitor<T, TResult> spanVisitor) => spanVisitor(_array.AsSpan(start, length));
	}
}
