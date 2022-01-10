# Pando State Tree Guidelines

These guidelines serve to help you create state trees that will work nicely with Pando. They are not *strictly*
necessary, however they keep things consistent, and are required if you're using `Pando.SerializerSourceGenerator` to
generate Serializer/Deserializers for your state tree.

See concrete examples illustrating these guidelines in the [Guidelines Examples](GuidelinesExamples)

## (0) A Pando state tree node is *either* a branch or a blob

A Pando state tree is composed of nodes that can be either branches or blobs (leaf nodes).

- A branch node contains other nodes.
- A blob node contains raw data.

## (1) A branch should only contain other nodes

Branches can contain other branches, blobs, or any combination thereof, but it should not contain "raw", primitive data.

When represented internally, the node data for a branch will consist of hashes of its child nodes

## (2) A blob should only contain raw data

A blob is a "leaf" of the pando tree. A blob should only contain primitive types, or atomic data types that can be
serialized as a primitive data type. A blob should not contain compound data types.

Data within a blob is usually serialized with an implementation of `IPandoSerializer<T>`. Pando
provides `IPandoSerializer` implementations for many primitive data types (see
the [Pando.Serialization.PrimitiveSerializers](../src/Pando/Serialization/PrimitiveSerializers) namespace), including (
but not limited to) the build in numeric types, enums, and strings.

Examples of valid data types to be contained in a blob:

- Primitive value types such as `int`, `float`, `char`, `bool`, enums, and [other *built-in value
  types*](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types).
- Data types that represent a single value and can be serialized as a primitive type.
    - For instance `DateTime` can be serialized to a `long` using `DateTime.ToBinary()`, so it is a valid data type to
      be in a blob.
- Strings.

Examples of invalid data types to store in a blob:

- Arrays. *Typically*, arrays indicate a collection of blobs, and should therefore be a branch. Sometimes it might be
  useful to semantically think of an array as a single value, in which case it would make sense to write a
  custom `IPandoSerializer` implementation for that array.

When represented internally, the node data for a blob will consist of the byte representation of its data.

If you can guarantee that an array/string is of a fixed length, it may be ok to include it in a blob. However, this
array/string should probably be wrapped in a custom struct and would require a custom Serializer/Deserializer.

## (3) An array of primitive value types is a blob

When an array contains a primitive value types, it can be serialized as a blob itself. This includes things such
as `int[]`, but also `List<int>`, etc.

## (4) An array of blobs is a branch

When an array contains other blobs, that array will be serialized as a branch, with each element being its own blob.
