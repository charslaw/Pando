# Person Example

This person node is an example of a "leaf" node, which contains only primitive data types.

```c#
public record Person(
	string Name,
	string Email,
	DateTime DateOfBirth,
	Gender Gender,
	EyeColor EyeColor,
	short NumberOfFingers
);

public enum Gender : byte { Male, Female, Nonbinary, Unspecified }

public enum EyeColor : byte { Blue, Green, Brown, Hazel, Black, Other }
```

## Serialization

You can read the full serializer class [here](../ExamplesCode/Person.cs), however here we will just include the
implementation of the `Serialize` and `Deserialize` methods.

### Serialize

```c#
/// Converts the given person to binary representation (a byte array), submits it as a new node to the data sink,
/// then returns the hash of the newly added node, so that whatever node contains this can use the hash.
public ulong Serialize(Person obj, INodeDataSink dataSink)
{
    var (name, email, dob, gender, eyeColor) = obj;

    // Get the size necessary to serialize this person
    var numBytes = _stringSerializer.ByteCountForValue(name)
        + _stringSerializer.ByteCountForValue(email)
        + _dateOfBirthSerializer.ByteCountForValue(dob)
        + _genderSerializer.ByteCountForValue(gender)
        + _eyeColorSerializer.ByteCountForValue(eyeColor);

    // Allocate the byte buffer to use
    Span<byte> bytes = stackalloc byte[numBytes];

    // Write data to the buffer
    var writeBuffer = bytes;
    _stringSerializer.Serialize(name, ref writeBuffer);
    _stringSerializer.Serialize(email, ref writeBuffer);
    _dateOfBirthSerializer.Serialize(dob, ref writeBuffer);
    _genderSerializer.Serialize(gender, ref writeBuffer);
    _eyeColorSerializer.Serialize(eyeColor, ref writeBuffer);

    // Add the node to the data sink and return the node hash.
    return dataSink.AddNode(bytes);
}
```

### Deserialize

```c#
/// Gets each of the values out of the given bytes array.
/// Note that since this node does not contain other nodes, the dataSource parameter is not used.
/// The dataSource parameter is used for nodes that contain other nodes so that they can retrieve their child nodes from the data source.
public Person Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource)
{
    // get member data out of the given byte buffer
    var name = _stringSerializer.Deserialize(ref bytes);
    var email = _stringSerializer.Deserialize(ref bytes);
    var dob = _dateOfBirthSerializer.Deserialize(ref bytes);
    var gender = _genderSerializer.Deserialize(ref bytes);
    var eyeColor = _eyeColorSerializer.Deserialize(ref bytes);

    // instantiate a person with all of its members
    return new Person(name, email, dob, gender, eyeColor);
}
```

## Memory Layout

The example `PersonSerializer` lays out the properties of a `Person` in memory like so. Note that most of this behavior
is defined by the specific primitive serializers used, the node serializer itself just coordinates the primitive
serializes and decides what order to put each member into the buffer.

```
   Name Length      Name      Email Length      Email        Date Of Birth     ┌─ Gender (1 byte)
 ┌──(4 bytes)──┐ ┌(N bytes)─┐ ┌─(4 bytes)──┐ ┌─(E bytes)──┐ ┌───(8 bytes)────┐ ├─────┐ ┌─────┬─ Eye Color (1 byte)
┌───┬───┬───┬───┬───┬  ┬─────┬─────┬  ┬─────┬─────┬  ┬─────┬───────┬  ┬───────┬───────┬───────┐
│ 0 │ 1 │ 2 │ 3 │ 4 │~~│ 4+N │4+N+1│~~│4+N+4│4+N+5│~~│8+N+E│8+N+E+1│~~│8+N+E+8│8+N+E+9│8+N+E+9│
└───┴───┴───┴───┴───┴  ┴─────┴─────┴  ┴─────┴─────┴  ┴─────┴───────┴  ┴───────┴───────┴───────┘
```

*Where `N` is the length in bytes of the name property, and `E` is the length in bytes of the email property.*
