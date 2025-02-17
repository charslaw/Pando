using System;

namespace Pando.Exceptions;

public class HashNotFoundException(string message) : Exception(message);

public class NoRootSnapshotException : Exception;

public class AlreadyHasRootSnapshotException : Exception;

public class UnableToMergeException() : InvalidOperationException("No merger available to perform a merge operation.");
