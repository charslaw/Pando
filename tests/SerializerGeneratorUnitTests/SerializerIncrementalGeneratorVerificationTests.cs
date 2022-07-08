using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Pando.SerializerGenerator;
using SerializerGeneratorUnitTests.Utils;
using Xunit;

namespace SerializerGeneratorUnitTests;

public class SerializerIncrementalGeneratorVerificationTests
{
	private static GeneratorDriver CreateAndRunDriverForFile(string filename)
	{
		var baseCompilation = CompilationUtils.CreateCompilationWithFiles(filename);
		var serializerGen = new SerializerIncrementalGenerator();
		return CSharpGeneratorDriver.Create(serializerGen).RunGenerators(baseCompilation);
	}

	private static void AssertNoErrors(GeneratorDriverRunResult runResult) =>
		runResult.Results.Should()
			.AllSatisfy(
				result => result.Exception.Should().BeNull(),
				because: "the generator should not throw exceptions"
			);

	[Theory]
	[InlineData("TestClasses/InvalidBecauseAbstract.cs")]
	[InlineData("TestClasses/InvalidBecauseNotSealed.cs")]
	public void Should_produce_no_results_when_target_type_is_invalid(string filename)
	{
		var generatorDriver = CreateAndRunDriverForFile(filename);

		var runResult = generatorDriver.GetRunResult();
		AssertNoErrors(runResult);

		runResult.Results.Should().AllSatisfy(result => result.GeneratedSources.Should().BeEmpty(), because: "");
	}
}
