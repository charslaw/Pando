using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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

	/// <param name="runResult">The run result to output generated source for.</param>
	/// <param name="relativePath">The path relative to the project root that files should be written to.</param>
	/// <param name="caller">The method calling this method. The caller name is included in the output path.</param>
	public static void WriteGeneratedSourceToFiles(this GeneratorRunResult runResult, string relativePath, [CallerMemberName] string caller = "")
	{
		var dir = Path.DirectorySeparatorChar.ToString();
		if (dir == @"\") dir = @"\\";
		
		var match = new Regex(@$"(?<projPath>.*){dir}bin{dir}(.*)").Match(Directory.GetCurrentDirectory());

		if (!match.Success) return;

		var destinationDir = Path.Combine(match.Groups["projPath"].Value, relativePath, caller);
		if (runResult.Exception is null && runResult.Diagnostics.IsEmpty)
		{
			Directory.CreateDirectory(destinationDir);
		}
		else
		{
			Directory.Delete(destinationDir, true);
			return;
		}

		foreach (var generatedSource in runResult.GeneratedSources)
		{
			var filePath = Path.Combine(destinationDir, generatedSource.HintName);
			File.WriteAllText(filePath, generatedSource.SyntaxTree.ToString());
		}
	}
}
