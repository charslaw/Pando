using Microsoft.CodeAnalysis;

namespace Pando.SerializerGenerator.Utils;

internal static class SymbolHelpers
{
	/// Returns the attribute data for an attribute on the given symbol with the given fully qualified name, if it exists.
	public static AttributeData GetAttributeByFullName(this ISymbol symbol, string name)
	{
		foreach (var attr in symbol.GetAttributes())
		{
			var attrType = attr.AttributeClass;
			if (attrType is null) continue;

			var attrFullName = attrType.ToDisplayString(CustomSymbolDisplayFormats.FullyQualifiedTypeName);
			if (attrFullName == name) return attr;
		}

		return null;
	}
}
