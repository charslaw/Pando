namespace SerializerGeneratorUnitTests.TestClasses;

[Pando.SerializerGenerator.Attributes.GenerateNodeSerializerAttribute]
internal record struct ValidStruct(object ObjectProp, [property: Pando.SerializerGenerator.Attributes.Primitive] float FloatProp);
