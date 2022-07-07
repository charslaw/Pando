using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pando.SerializerGenerator.Utils;

namespace Pando.SerializerGenerator;

public static class CandidateTypeValidator
{
	/// Ensures that the candidate type is a valid target:
	///  - It is sealed
	///  - It has a parameterless constructor
	///
	/// Returns true if valid, or false if valid. If it is invalid, <paramref name="diagnostic"/> will be populated, otherwise it will be null.
	public static bool TypeIsValid(INamedTypeSymbol typeSymbol, out Diagnostic diagnostic)
	{
		if (!typeSymbol.IsSealed)
		{
			var loc = GetLocationOfDeclarationWithMarker(typeSymbol);
			diagnostic = Diagnostic.Create(DiagnosticDescriptors.TypeNotSealedDescriptor, loc, typeSymbol);
			return false;
		}

		bool hasValidCtor = false;
		foreach (var ctor in typeSymbol.Constructors)
		{
			if (ctor.Parameters.Length != 0) continue;

			hasValidCtor = true;
			break;
		}

		if (!hasValidCtor)
		{
			var loc = GetLocationOfDeclarationWithMarker(typeSymbol);
			diagnostic = Diagnostic.Create(DiagnosticDescriptors.TypeHasNoParameterlessCtorDescriptor, loc, typeSymbol);
			return false;
		}

		diagnostic = null;
		return true;
	}

	private static Location GetLocationOfDeclarationWithMarker(INamedTypeSymbol typeSymbol)
	{
		var typeDeclaration = typeSymbol
				.GetAttributeByFullName(SerializerIncrementalGenerator.MARKER_ATTRIBUTE)
				?.ApplicationSyntaxReference?.GetSyntax()
				?.Parent?.Parent; // Attribute > AttributeList > ClassDeclaration

		return (typeDeclaration as BaseTypeDeclarationSyntax)?.Identifier.GetLocation() ?? typeSymbol.Locations[0];
	}
}
