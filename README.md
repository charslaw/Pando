# Pando

Pando is a history-aware tree serialization system. A single Pando "instance" keeps track of the full history of changes
to a static data hierarchy.

Pando is intended for use cases where all of the data you want to save is stored in a single tree hierarchy and imposes
some limitations on the organization of the data.

It uses [Merkle trees](https://en.wikipedia.org/wiki/Merkle_tree) to store a complete history of a tree over a series of
snapshots while avoiding data duplication (each unique blob is stored only once and referenced by hash).

## Usage

The entry point to Pando is the `PandoSaver<T>` class. Each `PandoSaver<T>` specifies the type of the root of the tree
hierarchy that it is intended to serialize. The public interface surface is defined in `IPandoSaver<T>`.

### Creating a `PandoSaver`

In order to create a `PandoSaver`, you must define a *repository* and a *serializer*. The *repository* determines the
destination to which data will be saved, while the *serializer* defines how to convert the tree into binary data.

The *repository* is of type `IPandoRepository` and the Pando library offers two built in options to fulfill this
dependency  (see [repositories](#repositories)). The *serializer* must be user defined as the logic will be specific to
the type that you are serializing (see [serializers](#serializers)).

```c#
// Create a new blank PandoSaver
var myPandoSaver = new PandoSaver<MyTreeRoot>(new InMemoryRepository(), new MyTreeRootSerializer());
```

### Saving to a `PandoSaver`

Once you have your `PandoSaver`, you can save a snapshot of your tree using `PandoSaver.SaveSnapshot`:

```c#
ulong SaveSnapshot(T tree);
```

`SaveSnapshot` takes in an instance of the root tree node to save and returns a `ulong` hash referring to the snapshot
that was just saved.

```c#
var snapshotHash = myPandoSaver.SaveSnapshot(myTreeRoot);
```

### Getting data from a `PandoSaver`

You can retrieve the tree state at a particular snapshot by calling `PandoSaver.GetSnapshot`:

```c#
T GetSnapshot(ulong hash);
```

`GetSnapshot` takes in a `ulong` snapshot hash and returns an instance of the root node of the tree at that snapshot. If
the snapshot doesn't exist within the repository, it will throw a `HashNotFoundException`.

```c#
var myTreeRootAtSnapshot = myPandoSaver.GetSnapshot(hash);
```

### Getting the full history from a `PandoSaver`

You can retrieve the entire history of snapshots by calling `PandoSaver.GetSnapshotTree`. This returns a
root `SnapshotTree` object, which is the root of a tree of snapshots. Each snapshot in the chain may have 0..many
children, which is represented by an `IImmutableSet<ulong>`.

```c#
SnapshotTree GetSnapshotTree();
```

Given this tree of snapshot hashes, you can use `GetSnapshot` to get the state at any point in the history.

## Details

### Repositories

An `IPandoRepository` defines how Pando will save data. Pando provides two implementations of `IPandoRepository`:

- `InMemoryRepository`: stores snapshots and tree data in-memory without persistence.
- `PersistenceBackedRepository`: stores snapshots and tree data in-memory, but also mirrors the data to 3 streams, one
  for each type of data Pando stores (the snapshot index, node index, and node data). These streams could be kind
  of `Stream`, but it would be typical for them to be file stream.

#### Storage in the Repository

A Pando repository stores 3 collections of data:

- **The Snapshot Index:** maps a snapshot hash to a `SnapshotData` struct, which itself is composed of a parent hash (
  which refers to this snapshot's parent snapshot) and a root node hash (which refers to the node index entry for the
  tree root node at this snapshot).
    - A snapshot's hash is computed by combining its two constituent hashes and hashing the resulting bytes.
- **The Node Index:** maps a node hash to a `DataSlice` struct, which is comprised of an offset into the stored node
  data and the length of this node's data.
    - Nodes in the index can take one of two semantic types: **leaf nodes (blobs)** and **branch nodes**. For a leaf
      node, the corresponding data in the stored node data is "real" data. On the other hand, for a branch node, the
      corresponding data in the stored node data is actually a collection of hashes referring to its child nodes in the
      index.
    - A node's hash in the index is computed from the bytes it refers to in the stored node data.
- **Stored Node Data:** this is a big blob of raw `bytes`. These bytes are created by the node serializer when
  serializing the node.
    - The interpretation of these bytes is undefined by Pando itself, it must be determined structurally based on what
      nodes refer to what slices of the data and the method by which the data was serialized.

A Pando repository has methods for writing snapshots and tree nodes to the repository, which take in the data comprising
the node or snapshot, and return the resulting hash within the corresponding index. It also has methods for reading
snapshots and tree nodes from the repository given the corresponding index hash.

### Serializers

The serializer given to the `PandoSaver` must implement `IPandoNodeSerializer<T>` where `T` is the same root node type
as the `PandoSaver` root type. This implementation must be provided by you because the logic for serialization will
depend upon the specific shape of your tree.

A Pando Node serializer is unique compared to other serializers in that it is designed to progressively save tree nodes
to the Pando repository starting from the leaf nodes (blobs) and returning the hash of the saved nodes so that the
hashes may be used to compose the parent nodes. Each Pando serializer defines how to serialize and deserialize one level
of the state tree.

The Serialize method is passed the node to serialize and also an instance of `IWritablePandoNodeRepository`. For branch
nodes, the serializer will delegate serialization of its contents to child serializers, then aggregate the hashes
returned from serializing its children and writing that to the repository.

The Deserialize method is passed a `ReadOnlySpan` of raw bytes and an instance of `IReadablePandoNodeRepository`. For
branch nodes, the serializer will parse out the hashes of its children, then delegate deserialization of the children by
using `GetNode` on the repository.

A pando node serializer will likely be composed of child serializers in a tree that mirrors the state tree, with each
serializer node being responsible for delegating to its children, then composing its own serialized form and serializing
itself.

To effectively author a Pando node serializer, you should have some knowledge of how data is stored in the repository.
See the section on [how data is stored in the repository](#storage-in-the-repository).

Also see `docs/PandoTreeExamples/README.md` for some guidelines on creating tree hierarchies that will play well with
Pando and its serialization model.
