using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorUnitTests.TestClasses;

[GenerateNodeSerializer]
internal sealed record InvalidBecauseMultipleMatchingConstructor(int Prop1, string Prop2)
{
	public InvalidBecauseMultipleMatchingConstructor(int prop1) : this(prop1, string.Empty) { }

	public void Deconstruct(out int prop1) { prop1 = Prop1; }
}
