# Serializers

## Node Serializers

A Pando node serializer is responsible for two things:

- Serialization: Converting a node's data to a `Span` of raw bytes, then submitting that data to the Pando data source
  via `AddNode`.
- Deserialization: Taking a given `ReadOnlySpan` of raw bytes and converting it back to the C# object those bytes
  represent.

Serializers implement the `IPandoNodSerializer<T>` interface, where `T` is the type of node that this serializer
serializes. An `IPandoNodeSerializer` implementation is unique to the type the node that it serializes, and thus it must
be specifically written for that node type, either manually or via the Pando source generator. Pando node serializers
are **nested** to mirror the structure of the state tree.

A Pando node serializer is unique compared to a traditional serializer in that Pando node serializer is responsible for
both creating the serialized form of the node as well as submitting the serialized bytes to the data source. This is
because when the node is submitted to the data source, the data source returns the id of the node, which can then
included in the data for the parent node.

Similarly, when deserializing, the incoming serialized node bytes can contain hashes of child nodes. These are used to
retrieve child's node data, which is passed to the child node serializer for deserialization.

To effectively author a Pando node serializer, you should have some knowledge of how data is stored in the data source
(see [Pando Data Sources](data-sources.md)).

## Primitive Serializers

A primitive serializer is a more traditional serializer that converts a single primitive data type to or from a byte
representation. Primitive serializers are used by node serializers to accomplish the actual task of serializing the data
that makes up the node.

Pando provides several built in primitive serializers for a number of dotnet types, including numeric types, enums,
date/time related types, and strings.

## What counts as a Node and what counts as a Primitive Data Type?

Primitive data types are simple, atomic data that cannot meaningfully be broken down into smaller data. Nodes are
containers of data, whether that be other nodes or primitive data. See [Data Organization](data-organization.md) for
more information.

## Pando Serializer Source Generator

*WIP*

Pando comes with a source generator that will create a hierarchy of node serializers to match your state tree. The
source generator will use the built in primitive serializers to serialize the base data, and you can manually write your
own primitive serializers for your own custom primitive types for the source generator to use.
