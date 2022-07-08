namespace SerializerGeneratorUnitTests.TestClasses;

[Pando.SerializerGenerator.Attributes.GenerateNodeSerializerAttribute]
internal sealed record ValidClass(int Prop, string Prop2)
{
	public ValidClass() : this(default, default!) { }
}
