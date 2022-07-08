using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SerializerGeneratorUnitTests.Utils;

public static class SourceGeneratorTestUtils
{
	public static GeneratorRunResult GenerateFromSource(this IIncrementalGenerator generator, string sourceCode)
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
		var references = AppDomain.CurrentDomain.GetAssemblies()
			.Where(assembly => !assembly.IsDynamic)
			.Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
			.Cast<MetadataReference>();

		var compOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
		var compilation = CSharpCompilation.Create("SourceGeneratorTests", new[] { syntaxTree }, references, compOptions);

		var driver = CSharpGeneratorDriver.Create(generator).RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

		return driver.GetRunResult().Results.Single();
	}

	public static GeneratorRunResult GenerateFromSourceFile(this IIncrementalGenerator generator, string sourceFilePath)
	{
		var source = File.ReadAllText(sourceFilePath);
		return generator.GenerateFromSource(source);
	}
}
