using System;
using Pando.DataSources;
using Pando.Repositories;
using Pando.Serialization;
using Pando.Serialization.Collections;

namespace ExamplesCode;

internal record Company(
	string CompanyName, // CompanyName is a primitive value
	Person[] Employees // Employees is a node; each employee is a leaf node (see the basic example)
);

internal class CompanySerializer : IPandoSerializer<Company>
{
	// Use default UTF8 serializer for the company name
	private readonly StringSerializer _companyNameSerializer = StringSerializer.UTF8;

	// NodeListSerializer is used to serialize enumerable types that implement the IList<> interface.
	private readonly ArraySerializer<Person> _employeesSerializer = new(
		new PersonSerializer() // The serializer that will be used to serialize the elements of the array
	);

	// The serialized size is the size that this node will occupy in the parent buffer. For a node like Company, this will just be a hash.
	public int SerializedSize => NodeId.SIZE;

	/// Writes the serialized value of the company name and employee array into a node and returns the hash of that node to the parent buffer.
	public void Serialize(Company obj, Span<byte> buffer, INodeDataStore dataStore)
	{
		// Allocate a buffer of the appropriate size to fit the company name and employees
		var employeesStart = _companyNameSerializer.SerializedSize;
		var totalSize = employeesStart + _employeesSerializer.SerializedSize;

		Span<byte> childBuffer = stackalloc byte[totalSize];

		// Serialize the company name. The default StringSerializer will save the name as a node and add the node's hash to childBuffer.
		_companyNameSerializer.Serialize(obj.CompanyName, childBuffer[..employeesStart], dataStore);
		// Serialize the company employees. The ArraySerializer will call the PersonSerializer for each person in the array, then save the returned
		// person node hashes into its own node, then add that node's hash to childBuffer.
		_employeesSerializer.Serialize(obj.Employees, childBuffer[employeesStart..totalSize], dataStore);

		// Add the serialized children to the data sink, copying the resulting node id into the input buffer.
		dataStore.AddNode(childBuffer, buffer);
	}

	/// Converts a company's binary representation to a Company instance.
	/// The name is deserialized via the string serializer.
	/// The ID of the employees array is read from the buffer, then the DeserializeFromHash extension method is called to retrieve the employees
	/// array binary representation from the data source and deserialize it into an array instance.
	public Company Deserialize(ReadOnlySpan<byte> readBuffer, IReadOnlyNodeDataStore dataStore)
	{
		var employeesStart = _companyNameSerializer.SerializedSize;
		var totalSize = employeesStart + _employeesSerializer.SerializedSize;

		// Retrieve the node data for this node
		Span<byte> nodeData = stackalloc byte[totalSize];
		dataStore.CopyNodeBytesTo(readBuffer, nodeData);

		// Deserialize company name
		var companyName = _companyNameSerializer.Deserialize(nodeData[..employeesStart], dataStore);
		// Deserialize Employees
		var employees = _employeesSerializer.Deserialize(nodeData[employeesStart..totalSize], dataStore);

		return new Company(companyName, employees);
	}
}
