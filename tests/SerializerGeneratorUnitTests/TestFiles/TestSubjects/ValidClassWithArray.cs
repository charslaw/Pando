using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorUnitTests.TestFiles.TestSubjects;

[GenerateNodeSerializer]
internal sealed record ValidClassWithArray(int[] intArray);
