using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorUnitTests.TestClasses;

[GenerateNodeSerializer]
internal struct InvalidBecauseNoMatchingConstructorStruct
{
	public int Prop { get; }

	public InvalidBecauseNoMatchingConstructorStruct() { Prop = default; }

	public void Deconstruct(out int prop) { prop = Prop; }
}
