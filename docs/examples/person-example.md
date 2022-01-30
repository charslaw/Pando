# Person Example

This person node is an example of a "leaf" node, which contains only primitive data types.

```c#
public record Person(
	string Name,
	DateTime DateOfBirth,
	Gender Gender,
	EyeColor EyeColor,
	short NumberOfFingers
);

public enum Gender : byte { Male, Female, Nonbinary, Unspecified }

public enum EyeColor : byte { Blue, Green, Brown, Hazel, Black, Other }
```

## Serialization

You can read the full serializer class [here](../ExamplesCode/Person.cs).

## Memory Layout

The example `PersonSerializer` lays out the properties of a `Person` in memory like so.
Note that most of this behavior is defined by the specific primitive serializers used,
the node serializer itself just coordinates the primitive serializes and decides what order to put each member into the buffer.

```
   Name Length      Name      Date Of Birth  ┌─ Gender (1 byte)
 ┌──(4 bytes)──┐ ┌(N bytes)─┐ ┌─(8 bytes)──┐ ├───┐ ┌────┬─ Eye Color (1 byte)
┌───┬───┬───┬───┬───┬  ┬─────┬─────┬  ┬─────┬─────┬──────┐
│ 0 │ 1 │ 2 │ 3 │ 4 │~~│ 4+N │4+N+1│~~│4+N+8│4+N+9│4+N+10│
└───┴───┴───┴───┴───┴  ┴─────┴─────┴  ┴─────┴─────┴──────┘
```

*Where `N` is the length in bytes of the name property.*
