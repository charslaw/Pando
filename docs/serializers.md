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
retrive child's node data, which is passed to the child node serializer for deserialization.

To effectively author a Pando node serializer, you should have some knowledge of how data is stored in the data source
(see [Pando Data Sources](data-sources.md)).
