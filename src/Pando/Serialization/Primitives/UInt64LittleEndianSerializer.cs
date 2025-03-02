using System;
using System.Buffers.Binary;
using Pando.DataSources;

namespace Pando.Serialization.Primitives;

/// Serializes/deserializes <c>ulong</c> values in little endian encoding (least significant byte first)
public class UInt64LittleEndianSerializer : IPandoSerializer<ulong>
{
	/// <summary>A global default instance for <see cref="UInt64LittleEndianSerializer"/></summary>
	public static UInt64LittleEndianSerializer Default { get; } = new();

	public int SerializedSize => sizeof(ulong);

	public void Serialize(ulong value, Span<byte> buffer, INodeDataStore dataStore) =>
		BinaryPrimitives.WriteUInt64LittleEndian(buffer, value);

	public ulong Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore) =>
		BinaryPrimitives.ReadUInt64LittleEndian(buffer);
}
