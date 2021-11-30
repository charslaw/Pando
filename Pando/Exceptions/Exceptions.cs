using System;

namespace Pando.Exceptions
{
    public class HashNotFoundException : Exception
    {
        public HashNotFoundException(string message) : base(message) { }
    }

    public class NoRootSnapshotException : Exception { }
}
