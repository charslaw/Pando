<img style="float: right" width="160px" height="160px" src="docs/noun-tree-4494965.svg" alt="Tree by Diky Setiawan from NounProject.com">

# Pando

Pando is a history-aware state tree serialization system. Using a Pando Repository, you can save a snapshot of your tree
hierarchy, and Pando will keep track of the history of snapshots, including branching histories and backtracking.

Pando is intended for use cases where all of the data you want to save is stored in a single tree hierarchy and imposes
some limitations on the organization of the data.

It uses [Merkle trees](https://en.wikipedia.org/wiki/Merkle_tree) to store a complete history of a tree over a series of
snapshots while avoiding data duplication (each unique blob is stored only once and referenced by hash).

## Usage

The entry point to Pando is the `PandoRepository<T>` class, where `T` is the type of the tree that you want to save in
this repository. The public interface surface is defined
in [`Pando.Repositories.IRepository<T>`](src/Pando/Repositories/IRepository.cs).

### Creating a `PandoRepository`

In order to create a `PandoRepository`, you must define a *data source* and a *serializer*. The *data source* determines
the destination to which data will be saved, while the *serializer* defines how to convert the tree into binary data.

The *data source* is of type `IDataSource` and the Pando library offers two built in options to fulfill this
dependency  (see [data sources](#data-sources)). The *serializer* must be user defined as the logic will be specific to
the type that you are serializing (see [serializers](#serializers)).

### Saving to a `PandoRepository`

Save a snapshot of your state tree:

```c#
ulong snapshotHash = myPandoRepository.SaveSnapshot(myTreeRoot);
```

The returned hash can be used to identify and retrieve the snapshot at a later time.

### Getting data from a `PandoRepository`

Get a previously saved snapshot:

```c#
var myTreeRootAtSnapshot = myPandoRepository.GetSnapshot(snapshotHash);
```

### Getting the full history from a `PandoRepository`

Get a tree representation of the full history of all snapshots:

```c#
SnapshotTree history = myPandoRepository.GetSnapshotTree();
```

`SnapshotTree` is a recursive data structure that contains the hash of a snapshot, and a set of child `SnapshotTree`s.

# Credits

- [Tree](https://thenounproject.com/icon/tree-4494965/) by Diky Setiawan from NounProject.com

## Details

### Serializers

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
