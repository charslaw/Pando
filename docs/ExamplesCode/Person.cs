using System;
using System.Buffers.Binary;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Repositories;
using Pando.Serialization;
using Pando.Serialization.Collections;
using Pando.Serialization.Primitives;
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
internal class PersonSerializer : IPandoSerializer<Person>
{
	// The Person serializer composes a number of primitive serializers for each of the Person class's members
	// We use some hard coded default serializers here, but these could also be injected through a constructor.

	// Use default UTF8 serializer for strings
	private readonly StringSerializer _nameSerializer = StringSerializer.UTF8;

	// Use default DateTime serializer which will encode the date as a long using ToBinary
	private readonly DateTimeToBinarySerializer _dateOfBirthSerializer = DateTimeToBinarySerializer.Default;

	// Use the SerializerFor factory method to create a serializer for Gender
	private readonly IPandoSerializer<Gender> _genderSerializer = EnumSerializer.SerializerFor<Gender>();

	// Use the SerializerFor factory method to create a serializer for EyeColor
	private readonly IPandoSerializer<EyeColor> _eyeColorSerializer = EnumSerializer.SerializerFor<EyeColor>();

	public int SerializedSize => NodeId.SIZE;

	/// Writes the properties of a person into a new node, then writes that node's hash into the given parent buffer.
	public void Serialize(Person obj, Span<byte> buffer, INodeDataStore dataStore)
	{
		var dobStart = _nameSerializer.SerializedSize;
		var genderStart = dobStart + _dateOfBirthSerializer.SerializedSize;
		var eyeColorStart = genderStart + _genderSerializer.SerializedSize;
		var totalSize = eyeColorStart + _eyeColorSerializer.SerializedSize;

		Span<byte> childBuffer = stackalloc byte[totalSize];

		var (name, dob, gender, eyeColor) = obj;

		_nameSerializer.Serialize(name, childBuffer[..dobStart], dataStore);
		_dateOfBirthSerializer.Serialize(dob, childBuffer[dobStart..genderStart], dataStore);
		_genderSerializer.Serialize(gender, childBuffer[genderStart..eyeColorStart], dataStore);
		_eyeColorSerializer.Serialize(eyeColor, childBuffer[eyeColorStart..totalSize], dataStore);

		dataStore.AddNode(childBuffer, buffer);
	}

	/// Gets each of the values out of the given bytes array.
	/// Note that since this node does not contain other nodes, the dataSource parameter is not used.
	public Person Deserialize(ReadOnlySpan<byte> bytes, IReadOnlyNodeDataStore dataStore)
	{
		var dobStart = _nameSerializer.SerializedSize;
		var genderStart = dobStart + _dateOfBirthSerializer.SerializedSize;
		var eyeColorStart = genderStart + _genderSerializer.SerializedSize;
		var totalSize = eyeColorStart + _eyeColorSerializer.SerializedSize;

		Span<byte> childBuffer = stackalloc byte[totalSize];
		dataStore.CopyNodeBytesTo(bytes, childBuffer);

		var name = _nameSerializer.Deserialize(childBuffer[..dobStart], dataStore);
		var dob = _dateOfBirthSerializer.Deserialize(childBuffer[dobStart..genderStart], dataStore);
		var gender = _genderSerializer.Deserialize(childBuffer[genderStart..eyeColorStart], dataStore);
		var eyeColor = _eyeColorSerializer.Deserialize(childBuffer[eyeColorStart..totalSize], dataStore);

		return new Person(name, dob, gender, eyeColor);
	}
}
