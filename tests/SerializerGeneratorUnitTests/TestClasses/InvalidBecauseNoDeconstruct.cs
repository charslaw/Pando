using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorUnitTests.TestClasses;

[GenerateNodeSerializer]
internal sealed class InvalidBecauseNoDeconstruct { }
