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

## What's in a `Repository`?

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

## Serializers

Serialization in Pando is achieved through implementations of the `IPandoSerializer<T>` interface.
The serializer is responsible for serializing a node in the state tree into a given byte buffer,
deserializing from a byte buffer back into the original form,
and also performing merge operations on serialized byte buffers.

Pando serializers are usually composed into a tree of serializers that mirrors the state tree being serialized.

Pando serializers can be broadly split into two kinds: node serializers and inline serializers.
Both kinds implement `IPandoSerializer`; the difference is in implementation.

- Inline serializers are characterized by serializing their data directly into the given byte buffer.
- Node serializers instead serialize their data into a separate buffer, submit that buffer to the Node vault as its own node,
  and then save the returned node ID into the given byte buffer.

To illustrate the difference, imagine we have a type `record Vector3(double X, double Y, double Z);`.
We'll write a inline and node serializer implementation of the `Serialize` method to compare the approaches.

```csharp
// Inline serializer implementation
public void Serialize(Vector3 value, Span<byte> buffer, INodeVault nodeVault)
{
    BinaryPrimitives.WriteDoubleLittleEndian(buffer[..8], value.X);
    BinaryPrimitives.WriteDoubleLittleEndian(buffer[8..16], value.Y);
    BinaryPrimitives.WriteDoubleLittleEndian(buffer[16..24], value.Z);
}

// Node serializer implementation
public void Serialize(Vector3 value, Span<byte> buffer, INodeVault nodeVault)
{
    Span<byte> data = stackalloc byte[sizeof(double) * 3];

    BinaryPrimitives.WriteDoubleLittleEndian(data[..8], value.X);
    BinaryPrimitives.WriteDoubleLittleEndian(data[8..16], value.Y);
    BinaryPrimitives.WriteDoubleLittleEndian(data[16..24], value.Z);

    NodeId nodeId = nodeVault.AddNode(data);
    nodeId.CopyTo(buffer);
}
```

The inline serializer simply writes the x, y, and z components directly into the given byte buffer.
Meanwhile, the node serializer is more complicated. It first allocates its own data buffer,
writes the x, y, and z components into this allocated buffer, adds the allocated buffer to the node vault,
and writes the node id to the given byte buffer.

While the inline serializer is simple and easy to write, it can be suboptimal for large data structures because
the serialized data isn't split into nodes which can be persisted individually.
If your entire state object was serialized inline with inline serializers, every snapshot would create a full
copy of the serialized data, whereas if components of the state are split into individual nodes,
if a node doesn't change from snapshot to snapshot, it won't save any duplicate data for that node!

### Choosing the right serializer and organizing your data

Deciding how your data is organized and serialized is an important decision when working with Pando.
Should a piece of data be serialized inline or as a separate node? What is the optimal node structure to use?

Here are some guidelines for deciding whether to serialize inline or as a node:

- If the serialized size of the data is less than or equal to the size of a NodeId (8 bytes), always serialize inline.
- If the serialized size is greater than 8 bytes, consider serializing as a node.
  - If the data (_or_ its siblings' data) is expected to change more than a few times over the lifetime of the
    repository, it is likely more optimal to serialize as a node so that it is serialized independent of its siblings.
  - If the data (_and_ its siblings' data) changes very infrequently it may be more optimal to serialize inline
    to avoid the overhead of storing the node index entry for the data.

When it comes to the structure of nodes to use, use the following guidelines:

- Isolate data that changes infrequently from data that changes frequently.
  - For example, if you are making a game and have a bunch of enemies whose stats must be serialized,
    you might have a class `record Enemy(string Name, Color Color, int Strength, Vector3 Position)` stored in an `Enemy[] Enemies` array.
    This can be suboptimal if the position changes more frequently than the other data, since each time the position changes,
    a new copy of the Enemy node must be serialized (_even_ if the position is stored as a node, since the node id must be updated).
  - To remedy this, consider splitting those infrequently updated data into their own node,
    `record EnemyStats(string Name, Color Color, int Strength)`, with its own node serializer,
    and organize in one of the following two ways:
    - Include `EnemyStats` in `Enemy`: `record Enemy(EnemyStats Stats, Vector3 Position)`. The `Enemies` array remains unchanged.
    - Have two arrays: `EnemyStats[] Enemies` and `Vector3[] EnemyPositions`.
  - The former option is better than the original example. However, it is not as good as the latter option since each
    time an enemy's position changes, the `Enemy` needs to be serialized again, and while it doesn't directly include
    the enemy stats data, it includes the node id of the enemy stats data, and a separate copy of the enemy stats node
    id will be included in each copy of the enemy node. Storing them in separate arrays minimizes the number of
    duplicate node ids that need to be stored, since the changing node ids (for the enemy positions) are isolated from
    the unchanging node ids (for the enemy stats) in separate array nodes.
- Keep data that changes together in the same node.
  - Continuing with the enemy example above, say you are adding the ability for enemies to have a rotation as well as
    position. If the position and rotation change together, it makes sense to store them inline within the same node.
    However, if they often change independent of one another, it makes sense to store them in separate nodes, so that
    changing the position does not also save a copy of the rotation and vice versa.

<!--
TODO
- Configuration
- Describe Serializers in more detail
  - Provide overview of built-in serializers
  - When to serialize as a node vs inline
  - Writing your own serializers
  - How do the NodeVault and Serializers interact
-->
