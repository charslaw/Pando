using Microsoft.CodeAnalysis;

namespace Pando.SerializerGenerator.Utils;

public static class CustomSymbolDisplayFormats
{
	/// Displays a type with all namespaces, but not the global namespace
	public static readonly SymbolDisplayFormat FullyQualifiedTypeName = new(
		globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
		typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
		genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
	);

	/// Displays a type with containing types, but not namespaces.
	public static readonly SymbolDisplayFormat NestedTypeName = new(
		typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
		genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
	);
}
