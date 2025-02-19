using System;
using System.Linq;
using Pando.DataSources.Utils;

namespace PandoTests.Utils;

internal static class ArrayX
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

	public static T[] ToArray<T>(this SpannableList<T> list)
	{
		var arr = new T[list.Count];
		list.CopyTo(..list.Count, arr);
		return arr;
	}
}
