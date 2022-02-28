using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorUnitTests.TestClasses;

[GenerateNodeSerializer]
internal sealed class ValidClass
{
	public int Prop { get; }

	public ValidClass(int prop) { Prop = prop; }

	public void Deconstruct(out int prop) { prop = Prop; }
}
