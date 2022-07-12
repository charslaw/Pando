using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorIntegrationTests.TestSubjects;

/// Has one node child and one primitive child
[GenerateNodeSerializer]
public sealed record SimpleMixedNode(object Stuff, [property: Primitive] int Value);
