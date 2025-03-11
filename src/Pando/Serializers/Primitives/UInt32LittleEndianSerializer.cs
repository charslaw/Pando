using System;
using System.Buffers.Binary;
using Pando.Vaults;

namespace Pando.Serializers.Primitives;

/// Serializes/deserializes <c>uint</c> values in little endian encoding (least significant byte first)
public class UInt32LittleEndianSerializer : IPandoSerializer<uint>
{
	/// <summary>A global default instance for <see cref="UInt32LittleEndianSerializer"/></summary>
	public static UInt32LittleEndianSerializer Default { get; } = new();

	public int SerializedSize => sizeof(uint);

	public void Serialize(uint value, Span<byte> buffer, INodeVault nodeVault) =>
		BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);

	public uint Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault) =>
		BinaryPrimitives.ReadUInt32LittleEndian(buffer);
}
