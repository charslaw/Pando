using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Pando.SerializerGenerator.Utils;

namespace Pando.SerializerGenerator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SerializerAnalyzer : DiagnosticAnalyzer
{
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
		= ImmutableArray.Create(
			DiagnosticDescriptors.TypeNotSealedDescriptor,
			DiagnosticDescriptors.TypeHasNoAppropriateConstructor
		);

	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSymbolAction(AnalyzeTypes, SymbolKind.NamedType);
	}

	private static void AnalyzeTypes(SymbolAnalysisContext context)
	{
		if (context.Symbol is not INamedTypeSymbol typeSymbol) return;
		if (typeSymbol.GetAttributeByFullName(SerializerIncrementalGenerator.MARKER_ATTRIBUTE) is null) return;
		if (CandidateTypeValidator.TypeIsValid(typeSymbol, out var diagnostic)) return;

		if (diagnostic is not null) context.ReportDiagnostic(diagnostic);
	}
}
