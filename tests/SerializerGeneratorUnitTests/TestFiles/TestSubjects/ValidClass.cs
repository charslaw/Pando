using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorUnitTests.TestClasses;

[GenerateNodeSerializer]
internal sealed record ValidClass([property: Primitive] int PrimitiveProp, string NodeProp);
