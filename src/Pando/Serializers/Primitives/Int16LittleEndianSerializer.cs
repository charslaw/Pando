using System;
using System.Buffers.Binary;
using Pando.Vaults;

namespace Pando.Serializers.Primitives;

/// Serializes/deserializes <c>short</c> values in little endian encoding (least significant byte first)
public class Int16LittleEndianSerializer : IPandoSerializer<short>
{
	/// <summary>A global default instance for <see cref="Int16LittleEndianSerializer"/></summary>
	public static Int16LittleEndianSerializer Default { get; } = new();

	public int SerializedSize => sizeof(short);

	public void Serialize(short value, Span<byte> buffer, INodeVault nodeVault) =>
		BinaryPrimitives.WriteInt16LittleEndian(buffer, value);

	public short Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault) =>
		BinaryPrimitives.ReadInt16LittleEndian(buffer);
}
