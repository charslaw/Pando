using System;

namespace Pando.Serialization.NodeSerializers;

/// Defines methods for accessing and creating an array
public class ArrayAdapter<T> : IIndexableAdapter<T[], T>
{
	public int Count(T[] list) => list.Length;
	public T Get(T[] list, int index) => list[index];
	public T[] Create(ReadOnlySpan<T> items) => items.ToArray();
}
