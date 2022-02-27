using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pando.SerializerGenerator.Utils;

namespace Pando.SerializerGenerator;

/// Determines if a given type is a valid candidate for serializer generation
public static class CandidateTypeValidator
{
	private static readonly IEqualityComparer<ImmutableArray<IParameterSymbol>> parameterArrayEqualityComparer = new ParameterArrayEqualityComparer();

	public static ValidateResult ValidateCandidateType(ITypeSymbol typeSymbol, AttributeSyntax markerAttribute)
	{
		var markerLocation = markerAttribute?.GetLocation() ?? Location.None;

		if (!typeSymbol.IsSealed) return ValidateResult.Invalid.TypeNotSealed(typeSymbol, markerLocation);

		var members = typeSymbol.GetMembers();

		var deconstructorParamLists = new List<ImmutableArray<IParameterSymbol>>();
		var constructorParamLists = new List<ImmutableArray<IParameterSymbol>>();
		foreach (var symbol in members)
		{
			if (symbol.DeclaredAccessibility < Accessibility.Internal) continue;

			switch (symbol)
			{
				case IMethodSymbol { MethodKind: MethodKind.Constructor } constructor:
					constructorParamLists.Add(constructor.Parameters);
					break;
				case IMethodSymbol { MethodKind: MethodKind.Ordinary, Name: "Deconstruct", ReturnsVoid: true } deconstructor:
					var parameters = deconstructor.Parameters;
					if (parameters.Any() && parameters.All(static it => it.RefKind == RefKind.Out))
					{
						deconstructorParamLists.Add(parameters);
					}

					break;
			}
		}

		if (deconstructorParamLists.Count <= 0) return ValidateResult.Invalid.TypeDoesNotHaveDeconstructor(typeSymbol, markerLocation);

		var (matchCount, matchingParams) = MatchingParameterListsCount(deconstructorParamLists, constructorParamLists);

		if (matchCount == 0) return ValidateResult.Invalid.TypeDoesNotHaveMatchingDeconstructConstructorPair(typeSymbol, markerLocation);
		if (matchCount > 1) return ValidateResult.Invalid.TypeHasMoreThanOneMatchingDeconstructConstructorPair(typeSymbol, markerLocation);

		if (matchingParams is null) throw new Exception($"{nameof(matchCount)} is 1, but {nameof(matchingParams)} is null!");

		return new ValidateResult.Valid(matchingParams.Value);
	}

	private static (int count, ImmutableArray<IParameterSymbol>? matchingParams) MatchingParameterListsCount(
		List<ImmutableArray<IParameterSymbol>> deconstructorParamLists,
		List<ImmutableArray<IParameterSymbol>> constructorParamLists
	)
	{
		int matchCount = 0;
		ImmutableArray<IParameterSymbol>? matchingParams = null;
		foreach (var deCtorParams in deconstructorParamLists)
		{
			if (deCtorParams.Length <= 0) continue;

			foreach (var ctorParams in constructorParamLists)
			{
				if (!parameterArrayEqualityComparer.Equals(ctorParams, deCtorParams)) continue;

				matchCount++;
				matchingParams = deCtorParams;

				// If we find multiple matches, quit out early
				if (matchCount > 1) return (matchCount, null);
			}
		}

		return (matchCount, matchingParams);
	}

	public abstract record ValidateResult
	{
		private ValidateResult() { }

		public record Valid(ImmutableArray<IParameterSymbol> ParamArray) : ValidateResult;

		public record Invalid : ValidateResult
		{
			public Diagnostic Diagnostic { get; }
			private Invalid(Diagnostic diagnostic) { Diagnostic = diagnostic; }

			public static Invalid TypeNotSealed(ITypeSymbol typeSymbol, Location markerLocation)
				=> CreateDiagnostic(markerLocation, DiagnosticDescriptors.TypeNotSealedDescriptor, typeSymbol);

			public static Invalid TypeDoesNotHaveDeconstructor(ITypeSymbol typeSymbol, Location markerLocation)
				=> CreateDiagnostic(markerLocation, DiagnosticDescriptors.TypeDoesNotHaveDeconstructorDescriptor, typeSymbol);

			public static Invalid TypeDoesNotHaveMatchingDeconstructConstructorPair(ITypeSymbol typeSymbol, Location markerLocation)
				=> CreateDiagnostic(markerLocation, DiagnosticDescriptors.TypeDoesNotHaveMatchingDeconstructConstructorPairDescriptor, typeSymbol);

			public static Invalid TypeHasMoreThanOneMatchingDeconstructConstructorPair(ITypeSymbol typeSymbol, Location markerLocation)
				=> CreateDiagnostic(
					markerLocation,
					DiagnosticDescriptors.TypeHasMoreThanOneMatchingDeconstructConstructorPairDescriptor,
					typeSymbol
				);

			private static Invalid CreateDiagnostic(Location markerLocation, DiagnosticDescriptor descriptor, params object[] messageArgs)
			{
				var diagnostic = Diagnostic.Create(descriptor, markerLocation, messageArgs);

				return new Invalid(diagnostic);
			}
		}
	}
}
