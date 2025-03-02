using System;
using System.Buffers.Binary;
using Pando.DataSources;

namespace Pando.Serialization.Primitives;

public class HalfLittleEndianSerializer : IPandoSerializer<Half>
{
	/// <summary>A global default instance for <see cref="SingleLittleEndianSerializer"/></summary>
	public static HalfLittleEndianSerializer Default { get; } = new();

	public int SerializedSize => sizeof(short);

	public void Serialize(Half value, Span<byte> buffer, INodeDataStore dataStore) =>
		BinaryPrimitives.WriteHalfLittleEndian(buffer, value);

	public Half Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore) =>
		BinaryPrimitives.ReadHalfLittleEndian(buffer);
}
