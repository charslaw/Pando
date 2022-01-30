using System;
using System.Buffers.Binary;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers;
using Pando.Serialization.NodeSerializers.EnumerableFactory;
using Pando.Serialization.PrimitiveSerializers;

namespace ExamplesCode;

internal record Company(
	string CompanyName, // CompanyName is a primitive value
	Person[] Employees  // Employees is a node; each employee is a leaf node (see the basic example)
);

internal class CompanySerializer : INodeSerializer<Company>
{
	// Use default UTF8 serializer for the company name
	private readonly StringSerializer _companyNameSerializer = StringSerializer.UTF8;

	// NodeListSerializer is used to serialize enumerable types that implement the IList<> interface.
	private readonly NodeListSerializer<Person[], Person> _employeesSerializer = new(
		new PersonSerializer(), // The serializer that will be used to serialize the elements of the array
		new ArrayFactory<Person>() // An array implementation of IEnumerableFactory. This is used by the NodeListSerializer to create a new array instance during deserialization.
	);

	/// The size of this node depends on the specific `Company` being serialized, so NodeSize is null.
	public int? NodeSize => null;

	/// Returns the size of a specific `Company`
	/// The size of the company name can be obtained from the string serializer.
	/// Since `Employees` is a node, the binary representation of the Company contains its ID.
	public int NodeSizeForObject(Company obj)
	{
		var nameSize = _companyNameSerializer.ByteCountForValue(obj.CompanyName);

		return nameSize + sizeof(ulong);
	}

	/// Writes the binary representation of the company's name and the ID of the employees array node in the data sink.
	/// In order to serialize the employees array, this serializer calls the SerializeToHash extension method on the _employeesSerializer,
	/// which handles converting the array and its contents to binary representation, submitting them as nodes to the data sink, then returning
	/// the ID of the array so that we can reference it in the company's binary representation.
	public void Serialize(Company obj, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		_companyNameSerializer.Serialize(obj.CompanyName, ref writeBuffer);
		var employeesId = _employeesSerializer.SerializeToHash(obj.Employees, dataSink);
		BinaryPrimitives.WriteUInt64LittleEndian(writeBuffer, employeesId);
	}

	/// Converts a company's binary representation to a Company instance.
	/// The name is deserialized via the string serializer.
	/// The ID of the employees array is read from the buffer, then the DeserializeFromHash extension method is called to retrieve the employees
	/// array binary representation from the data source and deserialize it into an array instance.
	public Company Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		var companyName = _companyNameSerializer.Deserialize(ref readBuffer);
		var employeesId = BinaryPrimitives.ReadUInt64LittleEndian(readBuffer);
		var employees = _employeesSerializer.DeserializeFromHash(employeesId, dataSource);

		return new Company(companyName, employees);
	}
}
