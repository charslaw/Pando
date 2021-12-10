# Pando Tree Examples and Rules

## Guidelines for Designing Pando State Trees

These guidelines serve to help you create state trees that will work nicely with Pando. They are not *strictly*
necessary, however they keep things consistent, and are required if you're using `Pando.SerializerSourceGenerator` to
generate Serializer/Deserializers for your state tree.

### (0) A Pando state tree node is *either* a branch or a blob

A Pando state tree is composed of nodes that can be either branches and blobs.

- A branch node contains other nodes, either branches, blobs, or a combination thereof. A branch cannot contain raw
  data.
- A blob node contains raw data.

When represented internally, the node data for a branch will consist of hashes of its child nodes, while the node data
for a blob will consist of the byte representation of its data.

### (1) A blob must not contain a data type which is a branch or is itself a blob

A blob is a "leaf" of the pando tree. A blob should only contain primitive types, or atomic data types that can be
serialized as a primitive data type. A blob should not contain compound or variable size data types.

Examples of valid data types to be contained in a blob:

- Primitive value types such as `int`, `float`, `char`, `bool` (and [other *value
  types*](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types)).
- Data types that represent a single value and can be serialized to a primitive type.
	- For instance `DateTime` can be serialized to a `long` using `DateTime.ToBinary()`, so it is a valid data type to
	  be in a blob.

Examples of invalid data types to store in a blob:

- Arrays of any kind.
- Strings.

### (2) An array of primitive value types is a blob

When an array contains a primitive value types, it can be serialized as a blob itself. This includes things such
as `string` and `int[]`, but also `List<int>`, etc.

### (3) An array of blobs is a branch

When an array contains other blobs (for instance `string[]`), that array will be serialized as a branch, with each
element being its own blob.

## Example Project

*upcoming*
