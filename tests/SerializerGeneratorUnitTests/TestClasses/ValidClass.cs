using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorUnitTests.TestClasses;

[GenerateNodeSerializer]
internal sealed class ValidClass
{
	public int Prop { get; }

	public string Prop2 { get; }

	public ValidClass(int prop, string prop2)
	{
		Prop = prop;
		Prop2 = prop2;
	}

	public void Deconstruct(out int prop, out string prop2)
	{
		prop = Prop;
		prop2 = Prop2;
	}
}
