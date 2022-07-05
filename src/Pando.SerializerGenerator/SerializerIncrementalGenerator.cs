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
	private const string MARKER_ATTRIBUTE = "Pando.SerializerGenerator.Attributes.GenerateNodeSerializerAttribute";
	private const string PRIMITIVE_ATTRIBUTE = "Pando.SerializerGenerator.Attributes.PrimitiveAttribute";

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

					var attr = GetAttributeByFullName(type, MARKER_ATTRIBUTE);
					return attr is not null ? type : null;
				}
			)
			.Where(static sym => sym is not null)
			.Collect();

	/// Given a collection of types, return types that are valid candidates for a serializer to be generated.
	/// If a type is not a valid candidate, emits a diagnostic for that type.
	private static Dictionary<INamedTypeSymbol, IEnumerable<IPropertySymbol>> IdentifyValidTypes(
		ImmutableArray<INamedTypeSymbol> types,
		SourceProductionContext ctx
	)
	{
		var validTypes = new Dictionary<INamedTypeSymbol, IEnumerable<IPropertySymbol>>(SymbolEqualityComparer.IncludeNullability);

		foreach (var typeSymbol in types)
		{
			if (!typeSymbol.IsSealed)
			{
				var attrLocation = GetMarkerAttributeLocation(typeSymbol);
				ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.TypeNotSealedDescriptor, attrLocation, typeSymbol));
				continue;
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
				var attrLocation = GetMarkerAttributeLocation(typeSymbol);
				ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.TypeHasNoParameterlessCtorDescriptor, attrLocation, typeSymbol));
				continue;
			}

			validTypes.Add(typeSymbol, GetTypeProperties(typeSymbol));
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

	/// Returns the <see cref="Location"/> of the marker attribute on this type, or <see cref="Location.None"/> if it does not have the attribute.
	private static Location GetMarkerAttributeLocation(INamedTypeSymbol type)
	{
		var attribute = GetAttributeByFullName(type, MARKER_ATTRIBUTE);
		var attributeSyntax = attribute.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;
		return attributeSyntax?.GetLocation() ?? Location.None;
	}

	/// Returns the attribute data for an attribute on the given symbol with the given fully qualified name, if it exists.
	private static AttributeData GetAttributeByFullName(ISymbol symbol, string name)
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

	/// Given a collection of valid serializer generation candidate types, outputs generated serializers for each type.
	private static void WriteSources(Dictionary<INamedTypeSymbol, IEnumerable<IPropertySymbol>> types, SourceProductionContext spCtx)
	{
		foreach (var (type, properties) in types)
		{
			var paramList = new List<SerializedProp>();
			foreach (var prop in properties)
			{
				var paramTypeString = prop.Type.ToDisplayString(CustomSymbolDisplayFormats.NestedTypeName);
				var isPrimitive = GetAttributeByFullName(prop, PRIMITIVE_ATTRIBUTE) is not null;
				paramList.Add(new SerializedProp(paramTypeString, prop.Name, isPrimitive));
			}

			var text = GeneratedSerializerRenderer.Render(assembly, type, paramList);
			var fullyQualifiedTypeString = type.ToDisplayString(CustomSymbolDisplayFormats.FullyQualifiedTypeName);
			var filename = $"{fullyQualifiedTypeString}Serializer.g.cs";
			spCtx.AddSource(filename, SourceText.From(text, Encoding.UTF8));
		}
	}
}
