using System;
using Pando.DataSources;

namespace Pando.Serialization.NodeSerializers;

public interface INodeSerializer<T>
{
	/// The size in bytes of the data produced by this serializer, if known.
	/// If the size can be dynamic, should return null.
	/// <remarks>This should be constant after initialization. If the size of the data can vary, return null</remarks>
	int? NodeSize { get; }

	/// Returns the size of the buffer required to serialize the given object.
	int NodeSizeForObject(T obj);

	/// <summary>Produces a binary representation of a given node and writes it to the given <paramref name="writeBuffer"/></summary>
	/// <remarks>
	/// <p>If this node type contains other nodes, this method will first have to delegate to
	/// `INodeSerializer`s that are capable of serializing the given node's children, submit
	/// the children to the given data sink, then include the child nodes' IDs in the binary
	/// representation of the given node so they can be retrieved later when the node is
	/// deserialized.</p>
	/// <p>If this node contains primitive data, this method should include the binary
	/// representation of the primitive data in the binary representation of the given node,
	/// possibly via the use of an <c>IPrimitiveSerializer</c>.</p>
	/// </remarks>
	/// <example>
	/// Example 1: Basic implementation of <c>Serialize</c> with child node.
	/// Assume <c>MyNode</c> is an object with a single property <c>Child</c>, which is a node itself.
	/// <code>
	/// ulong Serialize(MyNode obj, Span&lt;byte&gt; writeBuffer, INodeDataSink dataSink)
	/// {
	///     var childNode = obj.Child;
	///	    ulong childId = childSerializer.SerializeToHash(childNode, dataSink);
	///	    BinaryPrimitives.WriteUInt64LittleEndian(writeBuffer, childId);
	/// }
	/// </code>
	/// Example 2: Basic implementation of Serialize with primitive member.
	/// Assume <c>MyLeafNode</c> is an object with a single int property <c>PrimitiveMember</c>.
	/// <code>
	/// ulong Serialize(MyLeafNode obj, Span&lt;byte&gt; writeBuffer, INodeDataSink dataSink)
	/// {
	///     var primitiveMember = obj.PrimitiveMember;
	///	    BinaryPrimitives.WriteInt32LittleEndian(writeBuffer, primitiveMember);
	/// }
	/// </code>
	/// </example>
	void Serialize(T obj, Span<byte> writeBuffer, INodeDataSink dataSink);

	/// <summary>Converts a binary representation of a node into an instance of that node.</summary>
	/// <returns>An instance of type T that is represented by the given binary representation.</returns>
	/// <remarks>
	/// <p>If this node type contains other nodes, this method will first have to deserialize
	/// the child nodes' IDs from the given binary representation and use those IDs to retrieve
	/// the binary representation of the child nodes from the data source, then delegate
	/// deserialization of the child nodes' binary representation to child deserializers, the
	/// result of which can be used to create an instance of the node type.</p>
	/// <p>If this node type contains primitive data, this method should deserialize the
	/// primitive data from the given binary representation and use it to create an instance
	/// of the node type.</p>
	/// </remarks>
	/// <example>
	/// Example 1: Basic implementation of Deserialize with child node.
	/// Assume <c>MyNode</c> is an object with a single property <c>Child</c>, which is a node itself.
	/// <code>
	/// MyNode Deserialize(ReadOnlySpan&lt;byte&gt; bytes, INodeDataSource dataSource)
	/// {
	///	    var childId = BinaryPrimitives.ReadUInt64LittleEndian(bytes);
	///	    var child = childSerializer.DeserializeFromHash(childId, dataSource);
	///	    return new MyNode(child);
	/// }
	/// </code>
	///
	/// Example 2: Basic implementation of Serialize with primitive member.
	/// Assume <c>MyLeafNode</c> is an object with a single int property <c>PrimitiveMember</c>.
	/// <code>
	/// MyLeafNode Deserialize(ReadOnlySpan&lt;byte&gt; bytes, INodeDataSource dataSource)
	/// {
	///	    var primitiveMember = BinaryPrimitives.ReadInt32LittleEndian(bytes);
	///	    return new MyLeafNode(primitiveMember);
	/// }
	/// </code>
	/// </example>
	T Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource);
}
