namespace Pando;

internal readonly record struct SnapshotData(ulong ParentHash, ulong RootNodeHash);

internal readonly record struct DataSlice(int Start, int Length);
