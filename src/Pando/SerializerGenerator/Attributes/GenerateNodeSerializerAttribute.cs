using System;

namespace Pando.SerializerGenerator.Attributes;

/// <summary>Marker Attribute to indicate that a Pando Node Serializer should be generated for the decorated type.</summary>
/// <remarks>
///     Note that this Attribute does nothing without the Pando.SerializerGenerator source generator.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class GenerateNodeSerializerAttribute : Attribute { }
