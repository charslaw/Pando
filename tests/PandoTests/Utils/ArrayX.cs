using Pando.DataSources.Utils;

namespace PandoTests.Utils;

internal static class ArrayX
{
	public static T[] CreateCopy<T>(this T[] arr) => (T[])arr.Clone();

	public static T[] ToArray<T>(this SpannableList<T> list)
	{
		var arr = new T[list.Count];
		list.CopyTo(..list.Count, arr);
		return arr;
	}
}
