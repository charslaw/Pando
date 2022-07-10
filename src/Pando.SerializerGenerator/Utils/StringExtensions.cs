namespace Pando.SerializerGenerator.Utils;

public static class StringExtensions
{
	/// Converts a PascalCase string to camelCase
	public static string ToCamelCase(this string str) => $"{char.ToLowerInvariant(str[0])}{str.Substring(1)}";
}
