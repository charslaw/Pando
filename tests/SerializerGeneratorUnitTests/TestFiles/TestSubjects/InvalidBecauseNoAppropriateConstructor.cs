namespace SerializerGeneratorUnitTests.TestFiles.TestSubjects;

[Pando.SerializerGenerator.Attributes.GenerateNodeSerializerAttribute]
public sealed class InvalidBecauseNoAppropriateConstructor
{
	public string Prop1 { get; }
	public string Prop2 { get; }

	public InvalidBecauseNoAppropriateConstructor(string prop1)
	{
		Prop1 = prop1;
		Prop2 = "some value";
	}
}
