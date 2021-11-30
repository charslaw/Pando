using System;
using System.Linq;

namespace PandoTests.Utils;

public static class ArrayX
{
	public static T[] CreateCopy<T>(this T[] arr) => (T[])arr.Clone();

	public static T[] Concat<T>(params T[][] arrays)
	{
		var totalLen = arrays.Sum(arr => arr.Length);

		var newArray = new T[totalLen];

		var head = 0;
		foreach (var array in arrays)
		{
			Array.Copy(array, 0, newArray, head, array.Length);
			head += array.Length;
		}

		return newArray;
	}
}