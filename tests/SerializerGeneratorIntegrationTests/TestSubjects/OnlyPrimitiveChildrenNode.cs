using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorIntegrationTests.TestSubjects;

[GenerateNodeSerializer]
public sealed record OnlyPrimitiveChildrenNode([property: Primitive] string Name, [property: Primitive] int Age);
