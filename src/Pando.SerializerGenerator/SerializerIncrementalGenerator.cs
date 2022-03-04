using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Pando.SerializerGenerator.Utils;

namespace Pando.SerializerGenerator;

[Generator]
public class SerializerIncrementalGenerator : IIncrementalGenerator
{
	private const string MARKER_ATTRIBUTE_SHORT = "GenerateNodeSerializer";
	private const string MARKER_ATTRIBUTE = "GenerateNodeSerializerAttribute";

	private static readonly AssemblyName assembly = typeof(SerializerIncrementalGenerator).Assembly.GetName();

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
#if DEBUG
		// if (!Debugger.IsAttached) Debugger.Launch();
#endif

		var symbols = CollectSymbols(context);

		context.RegisterSourceOutput(symbols,
			static (ctx, symbols) =>
			{
				if (symbols.IsDefaultOrEmpty) return;

				var typesAndCtorParams = CollectTypeCtorParams(symbols, ctx);

				WriteSources(typesAndCtorParams, ctx);
			}
		);
	}

	/// Collect types that have the generate serializer marker attribute *and* all Deconstruct methods.
	/// We collect deconstruct methods explicitly rather than going through the type directly because a deconstructor can be an extension method,
	/// which will not be on the type itself.
	/// We'll filter the deconstruct methods to only those on the types in question later.
	private static IncrementalValueProvider<ImmutableArray<INamedTypeSymbol>> CollectSymbols(IncrementalGeneratorInitializationContext context)
		=> context.SyntaxProvider.CreateSyntaxProvider(
				predicate: static (syntax, _) =>
				{
					if (syntax is not TypeDeclarationSyntax { AttributeLists.Count: > 0 } tds) return false;

					foreach (var attrList in tds.AttributeLists)
					{
						foreach (var attr in attrList.Attributes)
						{
							var attrName = attr.Name.ToString();
							if (attrName is MARKER_ATTRIBUTE_SHORT or MARKER_ATTRIBUTE) return true;
						}
					}

					return false;
				},
				transform: static (ctx, _) => (INamedTypeSymbol)ctx.SemanticModel.GetDeclaredSymbol(ctx.Node)
			)
			.Where(static sym => sym is not null)
			.Collect();

	/// Given a collection of types, return types that are valid candidates for a serializer to be generated along with the parameters on that type
	/// used to construct/deconstruct them.
	/// If a type is not a valid candidate, emits a diagnostic for that type.
	private static Dictionary<INamedTypeSymbol, ImmutableArray<IParameterSymbol>> CollectTypeCtorParams(ImmutableArray<INamedTypeSymbol> types,
		SourceProductionContext ctx)
	{
		var typesAndCtorParams = new Dictionary<INamedTypeSymbol, ImmutableArray<IParameterSymbol>>(SymbolEqualityComparer.IncludeNullability);

		foreach (var typeSymbol in types)
		{
			var attribute = typeSymbol.GetAttributes().First(static it => it.AttributeClass?.Name == MARKER_ATTRIBUTE);
			var attributeSyntax = attribute.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;

			var validationResult = CandidateTypeValidator.ValidateCandidateType(typeSymbol, attributeSyntax);

			switch (validationResult)
			{
				case CandidateTypeValidator.ValidateResult.Valid validResult:
					typesAndCtorParams[typeSymbol] = validResult.ParamArray;
					break;
				case CandidateTypeValidator.ValidateResult.Invalid invalidResult:
					ctx.ReportDiagnostic(invalidResult.Diagnostic);
					break;
			}
		}

		return typesAndCtorParams;
	}

	/// Given a collection of valid serializer generation candidate types, outputs generated serializers for each type.
	private static void WriteSources(Dictionary<INamedTypeSymbol, ImmutableArray<IParameterSymbol>> types, SourceProductionContext spCtx)
	{
		foreach (var (type, ctorParams) in types)
		{
			AddSerializerSource(type, ctorParams, spCtx);
		}
	}

	private static void AddSerializerSource(INamedTypeSymbol type, ImmutableArray<IParameterSymbol> ctorParams, SourceProductionContext spCtx)
	{
		var paramList = new List<Param>();
		foreach (var ctorParam in ctorParams)
		{
			var paramTypeString = ctorParam.Type.ToDisplayString(CustomSymbolDisplayFormats.NestedTypeName);
			paramList.Add(new Param(paramTypeString, ctorParam.Name));
		}

		var (filename, text) = GeneratedSerializerRenderer.Render(assembly, type, paramList);

		spCtx.AddSource(filename, SourceText.From(text, Encoding.UTF8));
	}
}
