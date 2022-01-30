# Data Organization

## Types of Data

Data in Pando takes two forms:

- **Primitives:** primitive data types are atomic types that can be represented natively in binary format.
    - This includes the
      [built in value types](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types)
      such as `int`, `bool`, `char`, enums, etc.
    - It also includes types that can be wholly represented as a built in value type,
      such as `TimeSpan` which can be represented as a `long` via the `Ticks` property.
    - `string` is a somewhat special case that is treated as a primitive type
      even though it is technically a compound data type.
      This is because a `string` often represents a single value,
      despite being made up of atomic pieces (`char`).
    - A hypothetical `Point2d` type that contains an x and y coordinate
      could be a good candidate for a primitive type.
- **Nodes:** nodes are containers for data,
  and can contain other nodes and/or primitive data types.
    - Nodes are compound data types.
      They can be either classes or structs.
    - Collections are nodes.
      Collections can contain either primitive data types (`int[]` or `List<int>`),
      or other nodes (`List<Person>`).
    - A hypothetical `Person` type that contains a name, birth date, and favorite color would be a node.

For information about how these types of data get serialized, see [Serializers](serializers.md).

## Guidelines and Recommendations

When building a state tree and corresponding serializer tree for use with Pando, bear in mind these guidelines.
In some cases, these guidelines will be enforced by the Pando serializer source generator if you choose to use that,
while others may not be enforced at all.

1. Prefer immutable types for state tree nodes when possible.
   Use `record` for custom classes and structs, and `readonly` for custom structs.
   Immutable state is more manageable as a general rule, and minimizes side effects.
   Additionally, mutating data changes its hash,
   which could potentially cause problems with data storage in Pando.
2. State tree nodes should have minimal behavior.
   Generally, they should be data containers only,
   possibly with methods for displaying or converting the data.
3. Generally, it is a good idea to follow the dotnet guidelines regarding
   [reference types vs value types](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct).
   To briefly summarize within the context of Pando:
   use reference types for nodes that contain other nodes
   and structs for nodes that only contain primitive data, unless the data is larger than ~16 bytes in size.
   Pando avoids boxing struct tree nodes.
