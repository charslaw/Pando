using System.Collections.Generic;

namespace Pando.SerializerGenerator.Utils;

public static class EnumerableExtensions
{
	public static string JoinToString<T>(this IEnumerable<T> enumerable, string separator) => string.Join(separator, enumerable);

	public static string JoinLines<T>(this IEnumerable<T> enumerable) => string.Join("\n", enumerable);
	public static string JoinInline<T>(this IEnumerable<T> enumerable) => string.Join(", ", enumerable);
	public static string JoinSum<T>(this IEnumerable<T> enumerable) => string.Join(" + ", enumerable);
}
