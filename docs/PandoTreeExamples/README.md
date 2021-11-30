# Pando Tree Examples and Rules

A Pando state tree is composed of branches and blobs. Blobs contain raw data, while branches contain blobs and other
branches. A branch cannot contain raw data. For a state tree to be "valid" as a Pando tree, it must follow these rules:

## (1) A blob must not contain a data type which is a branch or is itself a blob

A blob is a "leaf" of the pando tree. A blob should only contain primitive types, or atomic data types that can be
serialized as a primitive data type. A blob should not contain compound or variable size data types.

Examples of valid data types to be contained in a blob:

- Primitive value types such as `int`, `float`, `char`, `bool` (and [other *value
  types*](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types)).
- Data types that can be serialized to a primitive type. For instance `DateTime` can be serialized to a `long`
  using `DateTime.ToBinary()`, so it is a valid data type to be in a blob.

Examples of invalid data types:

- Arrays of any kind.
- Strings.

## (2) An array of primitive value types is a blob

When an array contains a primitive value types, it can be serialized as a blob itself. This includes things such
as `string` and `int[]`, but also `List<int>`, etc.

## (3) An array of blobs is a branch

When an array contains other blobs (for instance `string[]`), that array will be serialized as a branch, with each
element being its own blob.
