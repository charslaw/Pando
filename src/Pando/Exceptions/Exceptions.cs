using System;

namespace Pando.Exceptions;

public class HashNotFoundException(string message) : Exception(message);

public class NoRootSnapshotException : Exception;

public class AlreadyHasRootSnapshotException : Exception;
