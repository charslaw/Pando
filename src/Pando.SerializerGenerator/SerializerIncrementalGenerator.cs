using System.Collections.Generic;
using System.Collections.Immutable;
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
	public const string MARKER_ATTRIBUTE = "Pando.SerializerGenerator.Attributes.GenerateNodeSerializerAttribute";
	public const string PRIMITIVE_ATTRIBUTE = "Pando.SerializerGenerator.Attributes.PrimitiveAttribute";

	private static readonly AssemblyName assembly = typeof(SerializerIncrementalGenerator).Assembly.GetName();

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var candidateTypes = CollectCandidateTypes(context);

		context.RegisterSourceOutput(candidateTypes,
			static (ctx, candidates) =>
			{
				if (candidates.IsDefaultOrEmpty) return;

				var validCandidates = IdentifyValidTypes(candidates, ctx);

				WriteSources(validCandidates, ctx);
			}
		);
	}

	/// Collect types that have the generate serializer marker attribute
	private static IncrementalValueProvider<ImmutableArray<INamedTypeSymbol>> CollectCandidateTypes(IncrementalGeneratorInitializationContext context)
		=> context.SyntaxProvider.CreateSyntaxProvider(
				predicate: static (syntax, _) => syntax is TypeDeclarationSyntax { AttributeLists.Count: > 0 },
				transform: static (ctx, _) =>
				{
					if (ctx.SemanticModel.GetDeclaredSymbol(ctx.Node) is not INamedTypeSymbol type) return null;

					var attr = type.GetAttributeByFullName(MARKER_ATTRIBUTE);
					return attr is not null ? type : null;
				}
			)
			.Where(static sym => sym is not null)
			.Collect();

	/// Given a collection of types, return types that are valid candidates for a serializer to be generated.
	/// If a type is not a valid candidate, emits a diagnostic for that type.
	private static Dictionary<INamedTypeSymbol, IEnumerable<IPropertySymbol>> IdentifyValidTypes(ImmutableArray<INamedTypeSymbol> types, SourceProductionContext context)
	{
		var validTypes = new Dictionary<INamedTypeSymbol, IEnumerable<IPropertySymbol>>(SymbolEqualityComparer.IncludeNullability);

		foreach (var typeSymbol in types)
		{
			if (CandidateTypeValidator.TypeIsValid(typeSymbol, out _))
			{
				validTypes.Add(typeSymbol, GetTypeProperties(typeSymbol));
			}
		}

		return validTypes;
	}

	/// Returns an enumerable of property symbols on the given type that have accessible getters and setters
	private static IEnumerable<IPropertySymbol> GetTypeProperties(INamedTypeSymbol type)
	{
		foreach (var member in type.GetMembers())
		{
			if (member is IPropertySymbol
			    {
				    IsStatic: false, IsIndexer: false,
				    GetMethod.DeclaredAccessibility: Accessibility.Internal or Accessibility.Public,
				    SetMethod.DeclaredAccessibility: Accessibility.Internal or Accessibility.Public
			    } propertySymbol)
			{
				yield return propertySymbol;
			}
		}
	}

	/// Given a collection of valid serializer generation candidate types, outputs generated serializers for each type.
	private static void WriteSources(Dictionary<INamedTypeSymbol, IEnumerable<IPropertySymbol>> types, SourceProductionContext spCtx)
	{
		foreach (var (type, properties) in types)
		{
			var paramList = new List<SerializedProp>();
			foreach (var prop in properties)
			{
				var paramTypeString = prop.Type.ToDisplayString(CustomSymbolDisplayFormats.NestedTypeName);
				var isPrimitive = prop.GetAttributeByFullName(PRIMITIVE_ATTRIBUTE) is not null;
				paramList.Add(new SerializedProp(paramTypeString, prop.Name.ToCamelCase(), isPrimitive));
			}

			var text = GeneratedSerializerRenderer.Render(assembly, type, paramList);
			var fullyQualifiedTypeString = type.ToDisplayString(CustomSymbolDisplayFormats.FullyQualifiedTypeName);
			var filename = $"{fullyQualifiedTypeString}Serializer.g.cs";
			spCtx.AddSource(filename, SourceText.From(text, Encoding.UTF8));
		}
	}
}
