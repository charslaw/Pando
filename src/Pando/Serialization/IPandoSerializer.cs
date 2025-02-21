using System;
using Pando.DataSources;
using Pando.Serialization.Utils;

namespace Pando.Serialization;

/// <summary>
/// Interface for a type that serializes and deserializes <typeparamref name="T"/> into/out of a <see cref="IDataSource"/>.
/// </summary>
public interface IPandoSerializer<T>
{
	/// <summary>Returns the number of bytes that the serialized value will occupy in the parent buffer.</summary>
	/// <remarks>For a branch node in the tree, this will be the size of a hash, <c>sizeof(ulong)</c>, while for a primitive (leaf node),
	/// this will be the size of the serialized value itself.</remarks>
	int SerializedSize { get; }

	/// <summary>Produces a binary representation of a given node and writes it to the given <paramref name="buffer"/></summary>
	/// <remarks>
	///     <p>The implementation of Serialize is inherently recursive, because each node must delegate to serialize its child nodes.</p>
	///     <p>For a node with children, the implementation should serialize its children into an inner buffer that is saved as a node in the
	///     <paramref name="dataStore"/>, then copy the returned node hash to the given <paramref name="buffer"/>.</p>
	///     <p>For a primitive (leaf node), it should copy a binary representation of the primitive data into the given <paramref name="buffer"/>.</p>
	/// </remarks>
	/// <example>
	///     Example 1: Basic implementation of <c>Serialize</c> for a node with two child nodes, <c>A</c> and <c>B</c>.
	///     <code language="cs">
	///     ulong Serialize(MyNode obj, Span&lt;byte&gt; buffer, INodeDataSink dataSink)
	///     {
	///	        // Allocate a child buffer to store our children's data
	///         Span&lt;byte&gt; childBuffer = stackalloc byte[aSerializer.SerializedSize + bSerializer.SerializedSize];
	///
	///	        // Serialize children into the allocated buffer
	///         aSerializer.Serialize(obj.A, childBuffer, dataSink);
	///         bSerializer.Serialize(obj.B, childBuffer[aSerializer.SerializedSize..], dataSink);
	///
	///	        // Save serialized children in a node in the data sink
	///         var hash = dataSink.AddNode(childBuffer);
	///
	///	        // Write the saved node hash to the given parent buffer
	///	        BinaryPrimitives.WriteUInt64LittleEndian(buffer, hash);
	///     }
	///     </code>
	/// </example>
	/// <example>
	///     Example 2: Basic implementation of Serialize for a primitive type, <see cref="DateOnly"/>.
	///     <code language="cs">
	///     ulong Serialize(DateOnly obj, Span&lt;byte&gt; buffer, INodeDataSink dataSink)
	///     {
	///         // Since this is a primitive, we can just serialize directly into the given buffer.
	///	        BinaryPrimitives.WriteInt32LittleEndian(buffer, obj.DayNumber);
	///     }
	///     </code>
	/// </example>
	void Serialize(T value, Span<byte> buffer, INodeDataStore dataStore);

	/// <summary>Converts a binary representation of a node into an instance of that node.</summary>
	/// <returns>An instance of type T that is represented by the given binary representation.</returns>
	/// <remarks>
	/// <p>For a node with children, the implementation must pull its node hash from the given buffer, load the node data,
	/// then use child serializers to parse the node data appropriately, then return the constructed final value.</p>
	/// <p>For a primitive (leaf node), it should directly load and serialize the data from the parent buffer.</p>
	/// </remarks>
	/// <example>
	///     Example 1: Basic implementation of Deserialize for a node with two child nodes, <c>A</c> and <c>B</c>.
	///     <code>
	///     MyNode Deserialize(ReadOnlySpan&lt;byte&gt; buffer, INodeDataSource dataSource)
	///     {
	///         // Read the hash for this node from the buffer
	///	        var hash = BinaryPrimitives.ReadUInt64LittleEndian(buffer);
	///
	///         // Load node data
	///         Span&lt;byte&gt; childBuffer = stackalloc byte[aSerializer.SerializedSize + bSerializer.SerializedSize];
	///         dataSource.CopyNodeBytesTo(hash, childBuffer);
	///
	///         // Deserialize child values
	///         var a = aSerializer.Deserialize(childBuffer, dataSource);
	///         var b = bSerializer.Deserialize(childBuffer[aSerializer.SerializedSize..], dataSource);
	///
	///	        return new MyNode(a, b);
	///     }
	///     </code>
	/// </example>
	/// <example>
	///     Example 2: Basic implementation of Serialize for a primitive type, <see cref="DateOnly"/>.
	///     <code>
	///     DateOnly Deserialize(ReadOnlySpan&lt;byte&gt; buffer, INodeDataSource dataSource)
	///     {
	///	        return DateOnly.FromDayNumber(BinaryPrimitives.ReadInt32LittleEndian(buffer));
	///     }
	///     </code>
	/// </example>
	T Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore);

	/// <summary>Perform a 3-way merge from source buffer onto target buffer, with the result being output in base buffer.</summary>
	/// <remarks>
	/// 	<p>The input format of all buffers should match the format generated by serialization.</p>
	/// </remarks>
	/// <param name="baseBuffer">The common ancestor data of the 3-way merge and the destination of the output.</param>
	/// <param name="targetBuffer">The target of the 3-way merge.</param>
	/// <param name="sourceBuffer">The source of the 3-way merge.</param>
	/// <param name="dataStore"></param>
	void Merge(Span<byte> baseBuffer,
		ReadOnlySpan<byte> targetBuffer,
		ReadOnlySpan<byte> sourceBuffer,
		INodeDataStore dataStore) => MergeUtils.MergeInline(baseBuffer, targetBuffer, sourceBuffer);
}
