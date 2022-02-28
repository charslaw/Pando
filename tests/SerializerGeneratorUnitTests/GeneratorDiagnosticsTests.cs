using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Pando.SerializerGenerator;
using SerializerGeneratorUnitTests.Utils;
using Xunit;

namespace SerializerGeneratorUnitTests;

public class GeneratorDiagnosticsTests
{
	[Theory]
	[InlineData("TestClasses/InvalidBecauseAbstract.cs", "Pando_SG_0001")]
	[InlineData("TestClasses/InvalidBecauseNotSealed.cs", "Pando_SG_0001")]
	[InlineData("TestClasses/InvalidBecauseNoDeconstruct.cs", "Pando_SG_0002")]
	[InlineData("TestClasses/InvalidBecauseNoDeconstructStruct.cs", "Pando_SG_0002")]
	[InlineData("TestClasses/InvalidBecauseNoMatchingConstructor.cs", "Pando_SG_0003")]
	[InlineData("TestClasses/InvalidBecauseNoMatchingConstructorStruct.cs", "Pando_SG_0003")]
	[InlineData("TestClasses/InvalidBecauseMultipleMatchingConstructor.cs", "Pando_SG_0004")]
	public void Should_produce_correct_diagnostic_for_invalid_cases(string sourceFile, string expectedDiagnosticId)
	{
		var baseCompilation = CompilationUtils.CreateCompilationWithFiles(sourceFile);
		var serializerGen = new SerializerIncrementalGenerator();
		GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(serializerGen);

		generatorDriver.RunGeneratorsAndUpdateCompilation(baseCompilation, out _, out var diagnostics);

		using (new AssertionScope())
		{
			diagnostics.Length.Should().Be(1);
			diagnostics[0].Id.Should().Be(expectedDiagnosticId);
		}
	}
}
