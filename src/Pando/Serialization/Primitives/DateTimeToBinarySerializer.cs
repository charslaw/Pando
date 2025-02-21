using System;
using Pando.DataSources;

namespace Pando.Serialization.Primitives;

/// <summary>
/// Serializes/deserializes a <see cref="DateTime"/> via a <c>long</c> serializer
/// to serialize the <c>DateTime</c>'s <see cref="DateTime.ToBinary"/> encoding.
/// </summary>
public class DateTimeToBinarySerializer(IPandoSerializer<long> innerSerializer) : IPandoSerializer<DateTime>
{
	/// <summary>A global default instance for <see cref="DateTimeToBinarySerializer"/></summary>
	public static DateTimeToBinarySerializer Default { get; } = new(Int64LittleEndianSerializer.Default);

	public int SerializedSize { get; } = innerSerializer.SerializedSize;

	public void Serialize(DateTime value, Span<byte> buffer, INodeDataStore dataStore) => innerSerializer.Serialize(value.ToBinary(), buffer, dataStore);

	public DateTime Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore) => DateTime.FromBinary(innerSerializer.Deserialize(buffer, dataStore));
}
