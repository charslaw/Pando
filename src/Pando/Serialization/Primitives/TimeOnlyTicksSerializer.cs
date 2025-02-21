using System;
using Pando.DataSources;

namespace Pando.Serialization.Primitives;

/// <summary>
/// Serializes/deserializes a <see cref="TimeOnly"/> via an <c>long</c> serializer
/// to serialize the <c>TimeOnly</c>'s <see cref="TimeOnly.Ticks"/>.
/// </summary>
public class TimeOnlyTicksSerializer(IPandoSerializer<long> innerSerializer) : IPandoSerializer<TimeOnly>
{
	/// <summary>A global default instance for <see cref="TimeOnlyTicksSerializer"/></summary>
	public static TimeOnlyTicksSerializer Default { get; } = new(Int64LittleEndianSerializer.Default);

	public int SerializedSize { get; } = innerSerializer.SerializedSize;

	public void Serialize(TimeOnly value, Span<byte> buffer, INodeDataStore dataStore) => innerSerializer.Serialize(value.Ticks, buffer, dataStore);


	public TimeOnly Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore) => new(innerSerializer.Deserialize(buffer, dataStore));
}
