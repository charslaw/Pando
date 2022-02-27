using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorUnitTests.Utils;

public static class CompilationUtils
{
	public static Compilation CreateCompilationWithFiles(params string[] sources)
	{
		return CSharpCompilation.Create("compilation",
			sources.Select(it => CSharpSyntaxTree.ParseText(File.ReadAllText(it))),
			new[] { MetadataReference.CreateFromFile(typeof(GenerateNodeSerializerAttribute).Assembly.Location) },
			new CSharpCompilationOptions(OutputKind.ConsoleApplication)
		);
	}
}
