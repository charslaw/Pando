using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorIntegrationTests.TestSubjects;

[GenerateNodeSerializer]
public sealed record MultipleNodeChildrenNode(int[] Ages, object SomeObject);
