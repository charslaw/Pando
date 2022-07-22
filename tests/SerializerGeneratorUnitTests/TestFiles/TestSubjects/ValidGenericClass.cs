using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorUnitTests.TestFiles.TestSubjects;

[GenerateNodeSerializer]
internal sealed record ValidGenericClass<TGeneric>(TGeneric GenericThing);
