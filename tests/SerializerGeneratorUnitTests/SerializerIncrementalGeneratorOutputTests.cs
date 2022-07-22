using System.IO;
using System.Linq;
using FluentAssertions;
using Pando.SerializerGenerator;
using SerializerGeneratorUnitTests.Utils;
using Xunit;

namespace SerializerGeneratorUnitTests;

public class SerializerIncrementalGeneratorOutputTests
{
	[Theory]
	[InlineData("ValidClass", "ValidClassSerializer")]
	[InlineData("ValidStruct", "ValidStructSerializer")]
	[InlineData("ValidClassWithArray", "ValidClassWithArraySerializer")]
	[InlineData("ValidClassWithGeneric", "ValidClassWithGenericSerializer")]
	[InlineData("ValidGenericClass", "ValidGenericClassSerializer")]
	public void Should_produce_correct_output_when_type_is_valid(string inFileName, string outFileName)
	{
		var inFilePath = $"TestFiles/TestSubjects/{inFileName}.cs";
		var outFilePath = $"TestFiles/ExpectedOutput/{outFileName}.cs";

		var expected = File.ReadAllText(outFilePath);
		var runResult = new SerializerIncrementalGenerator().GenerateFromSourceFile(inFilePath);

		runResult.WriteGeneratedSourceToFiles("TestResults", inFileName);
		runResult.Exception.Should().BeNull();
		runResult.Diagnostics.Should().BeEmpty();

		var actual = runResult.GeneratedSources.Select(gs => gs.SyntaxTree.ToString());
		actual.Should().BeEquivalentTo(expected);
	}

	[Theory]
	[InlineData("TestFiles/TestSubjects/InvalidBecauseAbstract.cs", "PANDO_0001")]
	[InlineData("TestFiles/TestSubjects/InvalidBecauseNotSealed.cs", "PANDO_0001")]
	[InlineData("TestFiles/TestSubjects/InvalidBecauseNoAppropriateConstructor.cs", "PANDO_0002")]
	public void Should_produce_no_results_when_target_type_is_invalid(string filename, string diagnosticId)
	{
		var runResult = new SerializerIncrementalGenerator().GenerateFromSourceFile(filename);

		runResult.Exception.Should().BeNull();
		runResult.Diagnostics.Should().HaveCount(1).And.ContainSingle(d => d.Id == diagnosticId);
		runResult.GeneratedSources.Should().BeEmpty();
	}
}
