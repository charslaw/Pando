# Pando Tree Examples and Rules

## Restrictions and guidelines for state tree nodes

These guidelines serve to help you create state trees that will work nicely with Pando. They are not *strictly*
necessary, however they keep things consistent, and are required if you're using `Pando.SerializerSourceGenerator` to
generate Serializer/Deserializers for your state tree.

For a more detailed description of these guidelines, the [Guidelines page](Guidelines.md). For examples of valid and
invalid state trees in accordance with these guidelines, see the [Guidelines examples](GuidelinesExamples).

0. A Pando state tree node is *either* a branch or a blob.
1. A branch should only contain other nodes.
2. A blob should only contain raw data.
3. An array of primitive value types is a blob.
4. An array of blobs is a branch.

## Other Guidelines

1. Use immutable types for state tree nodes when possible. Use `record` for custom classes and structs, and `readonly`
   for custom structs. Immutable state is more manageable as a general rule, and minimizes side effects.
2. State tree nodes should have minimal inbuilt functionality. Generally, they should be data containers only, possibly
   with methods for displaying or converting the data.
3. Generally, it is a good idea to follow the dotnet guidelines
   regarding [reference types vs value types](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct)
   . To briefly summarize within the context of Pando: use reference types for branches and structs for blobs, unless
   the blob is larger than ~16 bytes in size. Pando avoids boxing tree nodes.

## Example Project

*upcoming*
