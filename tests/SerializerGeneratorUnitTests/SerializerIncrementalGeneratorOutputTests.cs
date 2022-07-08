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
	public void Should_produce_correct_output_when_type_is_valid(string inFile, string outFile)
	{
		var expected = File.ReadAllText(outFile);
		var runResult = new SerializerIncrementalGenerator().GenerateFromSourceFile(inFile);

		runResult.Exception.Should().BeNull();
		var actual = runResult.GeneratedSources.Select(gs => gs.SyntaxTree.ToString());
		actual.Should().BeEquivalentTo(expected);
	}
	
	[Theory]
	[InlineData("TestFiles/TestSubjects/InvalidBecauseAbstract.cs")]
	[InlineData("TestFiles/TestSubjects/InvalidBecauseNotSealed.cs")]
	public void Should_produce_no_results_when_target_type_is_invalid(string filename)
	{
		var runResult = new SerializerIncrementalGenerator().GenerateFromSourceFile(filename);

		runResult.Exception.Should().BeNull();
		runResult.GeneratedSources.Should().BeEmpty();
	}
}
