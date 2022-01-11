# Serializers

The serializer given to the `PandoRepository` must implement `IPandoNodeSerializer<T>` where `T` is the same root node
type as the `PandoRepository` root type. This implementation must be provided by you because the logic for serialization
will depend upon the specific shape of your tree.

A Pando Node serializer is unique compared to other serializers in that it is designed to progressively save tree nodes
to the Pando data source starting from the leaf nodes (blobs) and returning the hash of the saved nodes so that the
hashes may be used to compose the parent nodes. Each Pando serializer defines how to serialize and deserialize one level
of the state tree.

The Serialize method is passed the node to serialize and also an instance of `INodeDataSink`. For branch nodes, the
serializer will delegate serialization of its contents to child serializers, then aggregate the hashes returned from
serializing its children and writing that to the data sink.

The Deserialize method is passed a `ReadOnlySpan` of raw bytes and an instance of `INodeDataSource`. For branch nodes,
the serializer will parse out the hashes of its children, then delegate deserialization of the children by
using `GetNode` on the data source.

A pando node serializer will likely be composed of child serializers in a tree that mirrors the state tree, with each
serializer node being responsible for delegating to its children, then composing its own serialized form and serializing
itself.

To effectively author a Pando node serializer, you should have some knowledge of how data is stored in the data source.
See the section on [how data is stored in the data source](#storage-in-the-data-source).

Also see `docs/PandoTreeExamples/README.md` for some guidelines on creating tree hierarchies that will play well with
Pando and its serialization model.
