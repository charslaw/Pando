using System;

namespace Pando.Vaults.Utils;

/// A collection that you can add a span of items to, and copy data to a span.
internal class SpannableList<T>
{
	private const int EXPANSION_FACTOR = 2;

	public int Count => _head;

	private T[] _array;
	private int _head;

	public SpannableList(int capacity = 8)
	{
		_array = new T[capacity];
	}

	public SpannableList(ReadOnlySpan<T> initialValues)
	{
		var initialDataLength = initialValues.Length;
		_array = new T[initialDataLength];
		initialValues.CopyTo(_array.AsSpan(0, initialDataLength));
	}

	/// Adds a given span to the list.
	public Range AddSpan(ReadOnlySpan<T> sourceSpan)
	{
		lock (_array)
		{
			var sourceLength = sourceSpan.Length;

			EnsureCapacity(sourceLength);

			var start = _head;
			var destSpan = _array.AsSpan(start, sourceLength);
			sourceSpan.CopyTo(destSpan);
			_head += sourceLength;
			return (start.._head);
		}
	}

	/// Makes sure that the list has the given amount of head space to add items into
	private void EnsureCapacity(int length)
	{
		lock (_array)
		{
			var currentHeadspace = _array.Length - _head;
			if (currentHeadspace >= length)
				return;

			var minimumNewSize = _head + length;
			Array.Resize(ref _array, minimumNewSize * EXPANSION_FACTOR);
		}
	}

	/// Copy data from the list into a destination span.
	public void CopyTo(Range range, Span<T> destination)
	{
		lock (_array)
		{
			_array.AsSpan(range).CopyTo(destination);
		}
	}
}
