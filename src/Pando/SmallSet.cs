using System;
using System.Collections.Generic;

namespace Pando;

/// A SmallSet is a set that we expect to have zero or one elements most of the time, and thus is optimized for that case.
/// This will incur no allocations until the set grows to at least 2 elements, at which point a "real" set is allocated.
///
/// It's a bit cludgy, but for the sake of performance I think it works fine?
internal struct SmallSet<T> where T : struct
{
	private T? _single;
	private HashSet<T>? _set;

	public int Count => (_single is not null) ? 1 : _set?.Count ?? 0;

	public T Single => _single ?? throw new Exception("This SmallSet does not contain only a single element!");

	public IEnumerable<T> All => _set ?? throw new Exception("This SmallSet does not contain multiple elements!");

	public void Add(T item)
	{
		if (_single is null)
		{
			_single = item;
		}
		else
		{
			if (_set is null)
			{
				_set = new HashSet<T>
				{
					_single.Value,
					item,
				};
				_single = null;
			}
			else
			{
				_set.Add(item);
			}
		}
	}
}
