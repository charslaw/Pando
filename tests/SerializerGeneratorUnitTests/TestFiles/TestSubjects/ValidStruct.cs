using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorUnitTests.TestFiles.TestSubjects;

[GenerateNodeSerializer]
internal record struct ValidStruct(object ObjectProp, [property: Primitive] float FloatProp);
