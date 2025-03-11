using System;
using System.Buffers.Binary;
using Pando.Vaults;

namespace Pando.Serialization.Primitives;

/// Serializes/deserializes <c>ushort</c> values in little endian encoding (least significant byte first)
public class UInt16LittleEndianSerializer : IPandoSerializer<ushort>
{
	/// <summary>A global default instance for <see cref="UInt16LittleEndianSerializer"/></summary>
	public static UInt16LittleEndianSerializer Default { get; } = new();

	public int SerializedSize => sizeof(ushort);

	public void Serialize(ushort value, Span<byte> buffer, INodeVault nodeVault) =>
		BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);

	public ushort Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault) =>
		BinaryPrimitives.ReadUInt16LittleEndian(buffer);
}
