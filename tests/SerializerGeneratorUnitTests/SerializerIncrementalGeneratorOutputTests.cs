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
	[InlineData("TestFiles/TestSubjects/ValidClass.cs", "TestFiles/ExpectedOutput/ValidClassSerializer.cs")]
	[InlineData("TestFiles/TestSubjects/ValidStruct.cs", "TestFiles/ExpectedOutput/ValidStructSerializer.cs")]
	[InlineData("TestFiles/TestSubjects/ValidClassWithArray.cs", "TestFiles/ExpectedOutput/ValidClassWithArraySerializer.cs")]
	[InlineData("TestFiles/TestSubjects/ValidClassWithGeneric.cs", "TestFiles/ExpectedOutput/ValidClassWithGenericSerializer.cs")]
	public void Should_produce_correct_output_when_type_is_valid(string inFile, string outFile)
	{
		var expected = File.ReadAllText(outFile);
		var runResult = new SerializerIncrementalGenerator().GenerateFromSourceFile(inFile);

		runResult.WriteGeneratedSourceToFiles("TestResults");
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
