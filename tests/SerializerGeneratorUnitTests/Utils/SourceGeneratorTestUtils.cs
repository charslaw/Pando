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

	/// Creates a directory for the caller under the given path and saves all generated sources from the given run result to that directory.
	/// <param name="runResult">The run result to output generated source for.</param>
	/// <param name="relativePath">The path relative to the project root that files should be written to.</param>
	/// <param name="caller">The method calling this method. The caller name is included in the output path.</param>
	public static void WriteGeneratedSourceToFiles(this GeneratorRunResult runResult, string relativePath, [CallerMemberName] string caller = "")
	{
		var destinationDir = GetDestinationDir(relativePath, caller);

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

	/// Returns the destination to save generated source to. Will try to put the file in the project directory,
	/// otherwise it will be placed relative to the executable
	private static string GetDestinationDir(string relativePath, string caller)
	{
		var separator = Path.DirectorySeparatorChar.ToString();
		if (separator == @"\") separator = @"\\";

		var cwd = Directory.GetCurrentDirectory();
		var match = new Regex(@$"(?<projPath>.*){separator}bin{separator}(.*)").Match(cwd);

		return (!match.Success) ? cwd : Path.Combine(match.Groups["projPath"].Value, relativePath, caller);
	}
}
