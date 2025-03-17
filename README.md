![Pando Logo](./docs/pando-logo.svg)

# Pando

Pando is a tool for serializing snapshots of data.
It is based on the same data structure git uses to keep track of the history of snapshots: [Merkle trees](https://en.wikipedia.org/wiki/Merkle_tree).
This allows it to store a full history of saved data while minimizing data duplication,
and allows for rewinding to a previous point in history, branching into different timelines, and merging timelines.

Pando serialization relies on breaking down the data into nodes in a tree and serializing each node separately.
This allows it to avoid storing duplicate copies of nodes that have already been serialized.
Each discrete instance of saving data to the repository is known as a snapshot,
and each snapshot refers to a tree of serialized nodes, as well as its parent snapshot(s).
A tree of trees (or rather a DAG of trees).

Pando is designed to be fast and well optimized.
Pando serializers write and read directly to and from `Span<byte>` buffers,
and persisting the data to disk is lightning fast because the data structures are append-only.

## Usage

The entry point to Pando is the `PandoRepository<T>` class,
where `T` is the type of the tree that you want to save in this repository.
The public interface is defined in
[`Pando.Repositories.IPandoRepository<T>`](src/Pando/Repositories/IPandoRepository.cs).

Working with a `PandoRepository` is simple:

```csharp
// Save some initial data
MyStateTreeType initialStateTree = new MyStateTreeType(...);
SnapshotId initialSnapshot = pandoRepository.SaveRootSnapshot(stateTree);

// Update the state and save a new child snapshot of the initial
var newStateTree = ChangeStateTree(initialStateTree);
SnapshotId newSnapshot = pandoRepository.SaveSnapshot(newStateTree, initialSnapshot);

// Later, retrieve the state of a previous snapshot using its ID.
MyStateTreeType originalStateTree = pandoRepository.GetSnapshot(initialSnapshot);
```

## What's in a Repository?

A `PandoRepository` has 3 dependencies:

- `INodeVault`: this dependency is responsible for storing the state tree nodes and retrieving them.
  - Pando provides a built in `INodeVault` implementation: `MemoryNodeVault`.
    This implementation stores data in memory, but can also optionally take a `INodePersistor` which writes and reads data to disk. 
- `ISnapshotVault`: this dependency is responsible for storing the tree of snapshots and their relationships to one another.
  - Similar to `INodeVault`, `ISnapshotVault` has a built-in `MemorySnapshotVault` implementation,
    which can optionally use a `ISnapshotPersistor`.
- `IPandoSerializer<T>`: this dependency is responsible for converting your C# objects into raw bytes.
  - Pando provides fast generic implementations designed for serializing general-purpose nodes
    and optimized serializers for primitive data,
    but you can also provide your own custom implementations to optimize for your specific data and requirements.
  - While it is not strictly necessary, serialization in Pando is often itself performed by a tree of serializers.
    The built-in Pando serializers are all implemented to use composition of sub-serializers in a tree structure.
