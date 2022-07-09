using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pando.SerializerGenerator.Utils;

namespace Pando.SerializerGenerator;

public static class CandidateTypeValidator
{
	public static bool TypeIsValid(INamedTypeSymbol typeSymbol, out Diagnostic diagnostic)
	{
		if (!TypeIsSealed(typeSymbol, out diagnostic)) return false;

		var propList = SerializerIncrementalGenerator.CollectProperties(typeSymbol);

		if (!HasValidConstructor(typeSymbol, propList, out diagnostic)) return false;

		diagnostic = null;
		return true;
	}

	/// Verifies that the candidate type is sealed
	public static bool TypeIsSealed(INamedTypeSymbol typeSymbol, out Diagnostic diagnostic)
	{
		var typeSymbolFullyQualifiedString = typeSymbol.ToDisplayString(CustomSymbolDisplayFormats.FullyQualifiedTypeName);
		diagnostic = GetDiagnostic(DiagnosticDescriptors.TypeNotSealedDescriptor, typeSymbol, typeSymbolFullyQualifiedString);
		return typeSymbol.IsSealed;
	}

	/// Verifies that the given type has a constructor that accepts all of the serialized properties for the type.
	public static bool HasValidConstructor(INamedTypeSymbol typeSymbol, List<SerializedProp> propList, out Diagnostic diagnostic)
	{
		foreach (var ctor in typeSymbol.InstanceConstructors)
		{
			if (ctor.Parameters.Length != propList.Count) continue;

			var ctorIsMatch = true;
			for (int i = 0; i < propList.Count; i++)
			{
				if (!SymbolEqualityComparer.IncludeNullability.Equals(propList[i].Type, ctor.Parameters[i].Type))
				{
					ctorIsMatch = false;
					break;
				}
			}

			if (ctorIsMatch)
			{
				diagnostic = null;
				return true;
			}
		}

		var signature = string.Join(", ", propList.Select(prop => prop.Type.ToString()));
		var typeSymbolFullyQualifiedString = typeSymbol.ToDisplayString(CustomSymbolDisplayFormats.FullyQualifiedTypeName);
		var typeSymbolShortString = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
		diagnostic = GetDiagnostic(
			DiagnosticDescriptors.TypeHasNoAppropriateConstructor,
			typeSymbol,
			typeSymbolFullyQualifiedString,
			typeSymbolShortString,
			signature
		);
		return false;
	}

	private static Diagnostic GetDiagnostic(DiagnosticDescriptor descriptor, INamedTypeSymbol typeSymbol, params object[] diagnosticParams)
	{
		var typeDeclaration = typeSymbol
			.GetAttributeByFullName(SerializerIncrementalGenerator.MARKER_ATTRIBUTE)
			?.ApplicationSyntaxReference?.GetSyntax()
			?.Parent?.Parent; // Attribute > AttributeList > ClassDeclaration

		var loc = (typeDeclaration as BaseTypeDeclarationSyntax)?.Identifier.GetLocation() ?? typeSymbol.Locations[0];

		return Diagnostic.Create(descriptor, loc, diagnosticParams);
	}
}
