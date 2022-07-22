using System.Collections.Generic;
using Pando.SerializerGenerator.Attributes;

namespace SerializerGeneratorUnitTests.TestFiles.TestSubjects;

[GenerateNodeSerializer]
internal sealed record ValidClassWithGeneric(List<object> MyObjects);
