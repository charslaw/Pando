using System;
using Pando.DataSources;
using Pando.Serialization;
using Pando.Serialization.PrimitiveSerializers;

namespace ExamplesCode;

internal record Person(
	string Name,
	DateTime DateOfBirth,
	Gender Gender,
	EyeColor EyeColor
);

internal enum Gender { Male, Female, Nonbinary, Unspecified }

internal enum EyeColor { Blue, Green, Brown, Hazel, Black, Other }

/// Serializes and deserializes a Person node
internal class PersonSerializer : INodeSerializer<Person>
{
	// The Person serializer composes a number of primitive serializers for each of the Person class's members
	// We use some hard coded default serializers here, but these could also be injected through a constructor.

	// Use default UTF8 serializer for strings
	private readonly StringSerializer _nameSerializer = StringSerializer.UTF8;

	// Use default DateTime serializer which will encode the date as a long using ToBinary
	private readonly DateTimeToBinarySerializer _dateOfBirthSerializer = DateTimeToBinarySerializer.Default;

	// Use the SerializerFor factory method to create a serializer for Gender
	private readonly IPrimitiveSerializer<Gender> _genderSerializer = EnumSerializer.SerializerFor<Gender>();

	// Use the SerializerFor factory method to create a serializer for EyeColor
	private readonly IPrimitiveSerializer<EyeColor> _eyeColorSerializer = EnumSerializer.SerializerFor<EyeColor>();

	/// Node size is the size of this node in bytes if it is known, or null if it is variable.
	/// Strings can be variable length, so we don't know the size ahead of time.
	public int? NodeSize => null;

	/// Converts the given person to binary representation (a byte array), submits it as a new node to the data sink,
	/// then returns the hash of the newly added node, so that whatever node contains this can use the hash.
	public ulong Serialize(Person obj, INodeDataSink dataSink)
	{
		var (name, dob, gender, eyeColor) = obj;

		// Get the size necessary to serialize this person
		var numBytes = _nameSerializer.ByteCountForValue(name)
			+ _dateOfBirthSerializer.ByteCountForValue(dob)
			+ _genderSerializer.ByteCountForValue(gender)
			+ _eyeColorSerializer.ByteCountForValue(eyeColor);

		// Allocate the byte buffer to use
		Span<byte> bytes = stackalloc byte[numBytes];

		// Write data to the buffer
		var writeBuffer = bytes;
		_nameSerializer.Serialize(name, ref writeBuffer);
		_dateOfBirthSerializer.Serialize(dob, ref writeBuffer);
		_genderSerializer.Serialize(gender, ref writeBuffer);
		_eyeColorSerializer.Serialize(eyeColor, ref writeBuffer);

		// Add the node to the data sink and return the node hash.
		return dataSink.AddNode(bytes);
	}

	/// Gets each of the values out of the given bytes array.
	/// Note that since this node does not contain other nodes, the dataSource parameter is not used.
	/// The dataSource parameter is used for nodes that contain other nodes so that they can retrieve their child nodes from the data source.
	public Person Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource)
	{
		// get member data out of the given byte buffer
		var name = _nameSerializer.Deserialize(ref bytes);
		var dob = _dateOfBirthSerializer.Deserialize(ref bytes);
		var gender = _genderSerializer.Deserialize(ref bytes);
		var eyeColor = _eyeColorSerializer.Deserialize(ref bytes);

		// instantiate a person with all of its members
		return new Person(name, dob, gender, eyeColor);
	}
}
