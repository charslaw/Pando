using System.Collections.Generic;
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
		var candidateTypes = context.SyntaxProvider.CreateSyntaxProvider(
				predicate: static (syntax, _) => syntax is TypeDeclarationSyntax { AttributeLists.Count: > 0 },
				transform: static (ctx, _) =>
				{
					if (ctx.SemanticModel.GetDeclaredSymbol(ctx.Node) is not INamedTypeSymbol type) return null;

					var attr = type.GetAttributeByFullName(MARKER_ATTRIBUTE);
					if (attr is null) return null;
					
					if (!CandidateTypeValidator.TypeIsValid(type, out var __)) return null;
					
					return type;
				}
			)
			.Where(static sym => sym is not null)
			.Collect();

		context.RegisterSourceOutput(candidateTypes,
			static (spCtx, candidates) =>
			{
				if (candidates.IsDefaultOrEmpty) return;

				foreach (var typeSymbol in candidates)
				{
					WriteSourceForType(typeSymbol, spCtx);
				}
			}
		);
	}

	private static void WriteSourceForType(INamedTypeSymbol typeSymbol, SourceProductionContext spCtx)
	{
		var paramList = new List<SerializedProp>();
		foreach (var prop in typeSymbol.GetAccessibleProperties())
		{
			var paramTypeString = prop.Type.ToDisplayString(CustomSymbolDisplayFormats.NestedTypeName);
			var isPrimitive = prop.GetAttributeByFullName(PRIMITIVE_ATTRIBUTE) is not null;
			paramList.Add(new SerializedProp(paramTypeString, prop.Name.ToCamelCase(), isPrimitive));
		}

		var text = GeneratedSerializerRenderer.Render(assembly, typeSymbol, paramList);
		var fullyQualifiedTypeString = typeSymbol.ToDisplayString(CustomSymbolDisplayFormats.FullyQualifiedTypeName);
		var filename = $"{fullyQualifiedTypeString}Serializer.g.cs";
		spCtx.AddSource(filename, SourceText.From(text, Encoding.UTF8));
	}
}
