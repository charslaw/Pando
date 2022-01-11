# Pando Data Sources

An `IDataSource` defines how Pando will save data. Pando provides two implementations of `IDataSource`:

- `MemoryDataSource`: stores snapshots and tree data in-memory without persistence.
- `PersistenceBackedDataSource`: stores snapshots and tree data in-memory, but also mirrors the data to 4 streams, one
  for each type of data Pando stores (the snapshot index, leaf snapshot set, node index, and node data block). These
  streams could be any kind of `Stream`, but it would be typical for them to be file stream.

## Storage in the Data Source

A Pando data source stores 4 collections of data:

- **The Snapshot Index:** maps a snapshot hash to a `SnapshotData` struct, which is composed of this snapshot's parent's
  hash and a the hash of the root tree node at this snapshot.
    - A snapshot's hash is computed by combining its two constituent hashes and hashing the resulting bytes.
- **The Leaf Snapshot Set:** a set of snapshots that make up the "head" of all snapshot branches within the data source.
- **The Node Index:** maps a node hash to a `DataSlice` struct, which is comprised of an offset into the node data
  block and the length of this node's data.
    - A node's hash in the index is computed from the bytes it refers to in the node data block.
- **The Node Data Block:** this is a big chunk of raw `bytes`.
    - Node data can be comprised of raw primitive data and/or hashes of other nodes.
    - The interpretation of these bytes is undefined by Pando itself; it must be determined structurally based on which
      node index entry refers to a given slice of the data and the method by which the data was serialized.

A Pando data source has methods for writing snapshots and tree nodes to the data source, which take in the data
comprising the node or snapshot, and return the resulting hash within the corresponding index. It also has methods for
reading snapshots and tree nodes from the data source given the corresponding index hash.
