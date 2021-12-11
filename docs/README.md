# Pando Tree Examples and Rules

## Guidelines for Designing Pando State Trees

These guidelines serve to help you create state trees that will work nicely with Pando. They are not *strictly*
necessary, however they keep things consistent, and are required if you're using `Pando.SerializerSourceGenerator` to
generate Serializer/Deserializers for your state tree.

For a more detailed description of these guidelines, the [Guidelines page](Guidelines.md). For examples of valid and
invalid state trees in accordance with these guidelines, see the [Guidelines examples](GuidelinesExamples).

0. A Pando state tree node is *either* a branch or a blob.
1. A branch should only contain other nodes.
2. A blob should not contain another node.
3. An array of primitive value types is a blob.
4. An array of blobs is a branch.

## Example Project

*upcoming*
