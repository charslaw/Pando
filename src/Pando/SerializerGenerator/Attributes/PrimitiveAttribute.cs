using System;

namespace Pando.SerializerGenerator.Attributes;

/// <summary>Marker Attribute to indicate that a given property should be serialized as a primitive.</summary>
/// <remarks>
///     Note that this Attribute does nothing without the Pando.SerializerGenerator source generator, and requires the containing type to have the
///     <see cref="GenerateNodeSerializerAttribute"/> marker attribute.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class PrimitiveAttribute : Attribute { }
