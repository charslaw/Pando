using System;
using System.Buffers.Binary;
using Pando.Repositories;
using Pando.Serializers;
using Pando.Vaults;

namespace ExamplesCode;

internal record Vector3(double X, double Y, double Z);

/// Serializes a Vector3 inline into the given buffer
internal class Vector3InlineSerializer : IPandoSerializer<Vector3>
{
	// SerializedSize determines the size of the buffer required to serialize the data.
	// The byte buffer passed to Serialize and Deserialize should be this size.
	// Since we're serializing 3 doubles directly, the size of this node is 24 bytes (8 * 3)
	public int SerializedSize => sizeof(double) * 3;

	public void Serialize(Vector3 value, Span<byte> buffer, INodeVault nodeVault)
	{
		// Write the x, y, and z components directly to given buffer
		BinaryPrimitives.WriteDoubleLittleEndian(buffer.Slice(0, sizeof(double)), value.X);
		BinaryPrimitives.WriteDoubleLittleEndian(buffer.Slice(sizeof(double), sizeof(double)), value.Y);
		BinaryPrimitives.WriteDoubleLittleEndian(buffer.Slice(sizeof(double) * 2, sizeof(double)), value.Z);
	}

	public Vector3 Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault)
	{
		// Read the x, y, and z components directly from the given buffer
		var x = BinaryPrimitives.ReadDoubleLittleEndian(buffer.Slice(0, sizeof(double)));
		var y = BinaryPrimitives.ReadDoubleLittleEndian(buffer.Slice(sizeof(double), sizeof(double)));
		var z = BinaryPrimitives.ReadDoubleLittleEndian(buffer.Slice(sizeof(double) * 2, sizeof(double)));

		// Create the Vector3
		return new Vector3(x, y, z);
	}
}

/// Serializes a Vector3 into a new node and writes the node's ID to the given buffer
internal class Vector3NodeSerializer : IPandoSerializer<Vector3>
{
	// For the node serializer, the size of the buffer is the size of a NodeId, since the actual data will be
	// saved in a separate node, and its ID will be written to the given buffer.
	public int SerializedSize => NodeId.SIZE;

	public void Serialize(Vector3 value, Span<byte> buffer, INodeVault nodeVault)
	{
		// Allocate our own node buffer
		Span<byte> data = stackalloc byte[sizeof(double) * 3];

		// Write data to our allocated buffer
		BinaryPrimitives.WriteDoubleLittleEndian(data.Slice(0, sizeof(double)), value.X);
		BinaryPrimitives.WriteDoubleLittleEndian(data.Slice(sizeof(double), sizeof(double)), value.Y);
		BinaryPrimitives.WriteDoubleLittleEndian(data.Slice(sizeof(double) * 2, sizeof(double)), value.Z);

		// Add our allocated buffer to the vault as its own node, returning a node id
		NodeId nodeId = nodeVault.AddNode(data);

		// Write the returned node id to the given buffer
		nodeId.CopyTo(buffer);
	}

	public Vector3 Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault)
	{
		// Read the node id from the given buffer
		NodeId nodeId = NodeId.FromBuffer(buffer.Slice(0, sizeof(double)));

		// Allocate the space to store the serialized node
		Span<byte> data = stackalloc byte[sizeof(double) * 3];

		// Copy the node data to our allocated buffer
		nodeVault.CopyNodeBytesTo(nodeId, data);

		// Read the x, y, and z components from the node buffer
		var x = BinaryPrimitives.ReadDoubleLittleEndian(data.Slice(0, sizeof(double)));
		var y = BinaryPrimitives.ReadDoubleLittleEndian(data.Slice(sizeof(double), sizeof(double)));
		var z = BinaryPrimitives.ReadDoubleLittleEndian(data.Slice(sizeof(double) * 2, sizeof(double)));

		// Create the Vector3
		return new Vector3(x, y, z);
	}
}
