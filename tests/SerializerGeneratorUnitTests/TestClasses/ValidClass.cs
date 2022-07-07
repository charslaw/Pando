namespace SerializerGeneratorUnitTests.TestClasses;

[Pando.SerializerGenerator.Attributes.GenerateNodeSerializerAttribute]
internal sealed class ValidClass
{
	public int Prop { get; init; }
	public string Prop2 { get; init; }

	public ValidClass() { }
}
