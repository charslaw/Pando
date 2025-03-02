using System;
using Pando.Repositories;

namespace Pando.Exceptions;

// We only expect these exceptions to be created internally; they don't need the standard Exception constructors
#pragma warning disable CA1032

public class SnapshotIdNotFoundException(SnapshotId snapshotId, string paramName)
	: ArgumentException($"Could not find snapshot with id {snapshotId}", paramName);

public class NodeIdNotFoundException(NodeId nodeId, string paramName)
	: ArgumentException($"Could not find node with id {nodeId}", paramName);

public class NoRootSnapshotException() : Exception("No root snapshot exists.");

public class AlreadyHasRootSnapshotException() : Exception("A root snapshot already exists.");

public class InvalidMergeException(string message) : InvalidOperationException(message);

#pragma warning restore CA1032
