using System;

namespace Pando.Serialization.NodeSerializers;

/// Defines a way to read from and create indexable collections.
public interface IIndexableAdapter<TIndexable, T>
{
	int Count(TIndexable list);
	T Get(TIndexable list, int index);
	TIndexable Create(ReadOnlySpan<T> items);
}
