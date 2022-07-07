namespace SerializerGeneratorUnitTests.TestClasses;

[Pando.SerializerGenerator.Attributes.GenerateNodeSerializerAttribute]
internal struct ValidStruct
{
	public int Prop { get; init; } = default;

	public ValidStruct() {}
}
