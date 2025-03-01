using System;
using Pando.DataSources.Utils;
using Pando.Repositories;

namespace Pando.Exceptions;

public class SnapshotIdNotFoundException(SnapshotId snapshotId, string paramName)
	: ArgumentException($"Could not find snapshot with id {snapshotId}", paramName);

public class NodeIdNotFoundException(NodeId nodeId, string paramName)
	: ArgumentException($"Could not find node with id {nodeId}", paramName);


public class NoRootSnapshotException() : Exception("No root snapshot exists.");

public class AlreadyHasRootSnapshotException() : Exception("A root snapshot already exists.");

public class InvalidMergeException(string message) : InvalidOperationException(message);
