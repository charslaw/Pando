using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorUnitTests.TestClasses;

[GenerateNodeSerializer]
internal struct ValidStruct
{
	public int Prop { get; }

	public ValidStruct(int prop) { Prop = prop; }

	public void Deconstruct(out int prop) { prop = Prop; }
}
