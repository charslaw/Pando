#region Disable warnings

// This file contains example types that are never used, so we don't care if it gives us some warnings about that.
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedType.Local
// ReSharper disable CheckNamespace
// ReSharper disable ClassNeverInstantiated.Local
// Field never assigned
#pragma warning disable 649

#endregion

/// This shows an invalid example of a parent blob that contains a blob itself,
/// as well as an example of a potential solution to make the original example valid.
internal class ABlobWithAChildBlob
{
    private class Invalid
    {
        /// <summary>
        /// This parent tree node contains a primitive value type (<c>int MyInt</c>), thus it is a blob.
        /// It cannot simultaneously be a blob and contain another blob (<c>ChildBlob AChild</c>), thus this is an invalid example.
        /// </summary>
        private readonly struct ParentBlob
        {
            public readonly int MyInt;
            public readonly ChildBlob AChild;
        }

        private readonly struct ChildBlob
        {
            public readonly int AnotherInt;
            public readonly bool MyBool;
        }
    }

    private class Valid
    {
        /// This parent tree node is a branch because it does not contain a primitive value type.
        private class Parent
        {
            public readonly ChildBlobA MyChildBlobA;
            public readonly ChildBlobB MyChildBlobB;
        }

        private readonly struct ChildBlobA
        {
            // Split out MyInt from the invalid example into its own blob,
            // thus making the parent a "pure" branch node, which is valid.
            public readonly int MyInt;
        }

        private readonly struct ChildBlobB
        {
            public readonly int AnotherInt;
            public readonly bool MyBool;
        }
    }
}

/// This shows an example of an invalid blob that contains a string,
/// along with two examples of how this problem could be fixed.
internal class ABlobWithAString
{
    private class Invalid
    {
        private class Root
        {
            public readonly SomeBlob SomeBlob;
            public readonly Person Person;
        }

        /// Person contains a string and an int, making it invalid.
        private readonly struct Person
        {
            public readonly string Name;
            public readonly int Age;
        }

        private readonly struct SomeBlob
        {
            /* imagine that there is data in here; the details don't matter for this example */
        }
    }

    private class ValidOption1
    {
        private class Root
        {
            public readonly SomeBlob SomeBlob;
            public readonly Person Person;
        }

        /// Person is now a proper branch, containing a string and another blob
        private class Person
        {
            public readonly string Name;
            public readonly PersonAge Age;
        }

        private readonly struct PersonAge
        {
            /// Moved the Age int from Person into its own blob.
            /// This means Person only contains other blobs, and is thus a valid branch.
            public readonly int Age;
        }

        private readonly struct SomeBlob
        {
            /* imagine that there is data in here; the details don't matter for this example */
        }
    }

    private class ValidOption2
    {
        /// Person was "pulled up" into the root branch node,
        /// with the Person's name and age being separate blobs within the root branch.
        private class Root
        {
            public readonly SomeBlob SomeBlob;
            public readonly string PersonName;
            public readonly PersonAge PersonAge;
        }

        private readonly struct PersonAge
        {
            /// Like in ValidOption1, the primitive Age int was put in its own blob
            /// so that it could exist side by side with other blobs within a branch node.
            public readonly int Age;
        }

        private readonly struct SomeBlob
        {
            /* imagine that there is data in here; the details don't matter for this example */
        }
    }
}
