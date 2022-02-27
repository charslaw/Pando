using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorUnitTests.TestClasses;

[GenerateNodeSerializer]
internal sealed class InvalidBecauseNoMatchingConstructor
{
	public int Prop { get; }

	public InvalidBecauseNoMatchingConstructor() { Prop = default; }

	public void Deconstruct(out int prop) { prop = Prop; }
}
