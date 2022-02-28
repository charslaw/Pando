using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Pando.SerializerGenerator;
using SerializerGeneratorUnitTests.Utils;
using VerifyXunit;
using Xunit;

namespace SerializerGeneratorUnitTests;

[UsesVerify]
public class SerializerIncrementalGeneratorVerificationTests
{
	[Fact]
	public Task Should_produce_correct_output_for_valid_class()
	{
		var baseCompilation = CompilationUtils.CreateCompilationWithFiles("TestClasses/ValidClass.cs");
		var serializerGen = new SerializerIncrementalGenerator();
		GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(serializerGen);

		generatorDriver = generatorDriver.RunGenerators(baseCompilation);

		return Verifier.Verify(generatorDriver);
	}
}
