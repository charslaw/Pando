using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
					if (!CandidateTypeValidator.TypeIsSealed(typeSymbol, out var sealedDiagnostic))
					{
						spCtx.ReportDiagnostic(sealedDiagnostic);
						continue;
					}
					
					var propList = CollectProperties(typeSymbol);
					if (!CandidateTypeValidator.HasValidConstructor(typeSymbol, propList, out var ctorDiagnostic))
					{
						spCtx.ReportDiagnostic(ctorDiagnostic);
						continue;
					}

					var text = GeneratedSerializerRenderer.Render(assembly, typeSymbol, propList);
					var fullyQualifiedTypeString = typeSymbol.ToDisplayString(CustomSymbolDisplayFormats.FullyQualifiedTypeName);
					var filename = $"{fullyQualifiedTypeString}Serializer.g.cs";

					spCtx.AddSource(filename, text);
				}
			}
		);
	}

	/// Creates an initial list of properties for a type with no constructor param set,
	/// along with potential candidate names for matching constructor params
	public static List<SerializedProp> CollectProperties(INamedTypeSymbol typeSymbol)
	{
		var propList = new List<SerializedProp>();
		foreach (var member in typeSymbol.GetMembers())
		{
			if (member is IPropertySymbol
			    {
				    IsStatic: false, IsIndexer: false,
				    DeclaredAccessibility: Accessibility.Internal or Accessibility.Public,
				    GetMethod.DeclaredAccessibility: Accessibility.Internal or Accessibility.Public
			    } propertySymbol)
			{
				var name = propertySymbol.Name;
				var isPrimitive = propertySymbol.GetAttributeByFullName(PRIMITIVE_ATTRIBUTE) is not null;
				propList.Add(new SerializedProp((INamedTypeSymbol)propertySymbol.Type, name, isPrimitive));
			}
		}

		return propList;
	}
}
