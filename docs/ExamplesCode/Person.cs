using System;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers;
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

	/// Returns the size of a specific person.
	/// Since Person contains only primitive types, we just defer to each primitive serializer for the size of each member,
	/// then add them up.
	public int NodeSizeForObject(Person person)
	{
		var (name, dob, gender, eyeColor) = person;
		var nameSize = _nameSerializer.ByteCountForValue(name);
		var dobSize = _dateOfBirthSerializer.ByteCount ?? _dateOfBirthSerializer.ByteCountForValue(dob);
		var genderSize = _genderSerializer.ByteCount ?? _genderSerializer.ByteCountForValue(gender);
		var eyeColorSize = _eyeColorSerializer.ByteCount ?? _eyeColorSerializer.ByteCountForValue(eyeColor);

		return nameSize + dobSize + genderSize + eyeColorSize;
	}

	/// Writes the binary representation of the person's properties into the given write buffer.
	/// In this case, since Person only contains primitive data types,
	/// it just calls the serializer for each member and uses the given write buffer.
	/// Note that since this node does not contain other nodes, the dataSink parameter is not used.
	public void Serialize(Person obj, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		var (name, dob, gender, eyeColor) = obj;

		_nameSerializer.Serialize(name, ref writeBuffer);
		_dateOfBirthSerializer.Serialize(dob, ref writeBuffer);
		_genderSerializer.Serialize(gender, ref writeBuffer);
		_eyeColorSerializer.Serialize(eyeColor, ref writeBuffer);
	}

	/// Gets each of the values out of the given bytes array.
	/// Note that since this node does not contain other nodes, the dataSource parameter is not used.
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
