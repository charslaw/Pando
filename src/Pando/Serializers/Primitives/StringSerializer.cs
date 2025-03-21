using System;
using System.Text;
using Pando.Repositories;
using Pando.Vaults;

namespace Pando.Serializers.Primitives;

/// Serializes a string using a given encoding
public class StringSerializer(Encoding encoding) : IPandoSerializer<string>
{
	/// A default serializer for strings that uses the UTF8 encoding.
	public static StringSerializer UTF8 { get; } = new(Encoding.UTF8);

	/// A default serializer for strings that uses the ASCII encoding.
	public static StringSerializer ASCII { get; } = new(Encoding.ASCII);

	public int SerializedSize => NodeId.SIZE;

	public void Serialize(string value, Span<byte> buffer, INodeVault nodeVault)
	{
		ArgumentNullException.ThrowIfNull(nodeVault);

		var bytesSize = encoding.GetByteCount(value);
		Span<byte> elementBytes = stackalloc byte[bytesSize];
		encoding.GetBytes(value, elementBytes);

		nodeVault.AddNode(elementBytes, buffer);
	}

	public string Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault)
	{
		ArgumentNullException.ThrowIfNull(nodeVault);

		var nodeDataSize = nodeVault.GetSizeOfNode(buffer);
		Span<byte> elementBytes = stackalloc byte[nodeDataSize];
		nodeVault.CopyNodeBytesTo(buffer, elementBytes);
		return encoding.GetString(elementBytes);
	}
}
