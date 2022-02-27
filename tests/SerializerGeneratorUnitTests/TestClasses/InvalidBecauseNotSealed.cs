using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorUnitTests.TestClasses;

[GenerateNodeSerializer]
internal class InvalidBecauseNotSealed { }
