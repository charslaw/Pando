# Pando

<img style="float: right" width="160px" height="160px" src="docs/noun-tree-4494965.svg" alt="Tree by Diky Setiawan from NounProject.com">

Pando is a history-aware state tree serialization system.
Using a Pando Repository, you can save a snapshot of your tree hierarchy,
and Pando will keep track of the history of snapshots,
including branching histories and backtracking.

Pando is intended for use cases where all of the data you want to save is stored in a single tree hierarchy.

It uses [Merkle trees](https://en.wikipedia.org/wiki/Merkle_tree)
to store a complete history of a tree over a series of snapshots while avoiding data duplication
(each unique blob is stored only once and referenced by hash).

## Usage

The entry point to Pando is the `PandoRepository<T>` class,
where `T` is the type of the tree that you want to save in this repository.
The public interface surface is defined in
[`Pando.Repositories.IRepository<T>`](src/Pando/Repositories/IRepository.cs).

### Creating a `PandoRepository`

In order to create a `PandoRepository`, you must define a *data source* and a *serializer*.
The *data source* determines the destination to which data will be saved,
while the *serializer* defines how to convert the tree into binary data.

The *data source* is of type `IDataSource`
and the Pando library offers two built in options to fulfill this dependency
(see [data sources](docs/data-sources.md)).

The *serializer* must be user defined (or source generated)
as the logic will be specific to the type that you are serializing
(see [serializers](docs/serializers.md)).

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

`SnapshotTree` is a recursive data structure that contains the hash of a snapshot,
and a set of child `SnapshotTree`s.

# Credits

- [Tree](https://thenounproject.com/icon/tree-4494965/) by Diky Setiawan from NounProject.com
