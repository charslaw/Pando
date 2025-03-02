using System;
using Pando.DataSources;

namespace Pando.Serialization.Primitives;

/// <summary>
/// Serializes/deserializes a <see cref="DateOnly"/> via an <c>int</c> serializer
/// to serialize the <c>DateOnly</c>'s <see cref="DateOnly.DayNumber"/>.
/// </summary>
public class DateOnlyDayNumberSerializer(IPandoSerializer<int> innerSerializer) : IPandoSerializer<DateOnly>
{
	/// <summary>A global default instance for <see cref="DateOnlyDayNumberSerializer"/></summary>
	public static DateOnlyDayNumberSerializer Default { get; } = new(Int32LittleEndianSerializer.Default);

	public int SerializedSize { get; } = innerSerializer.SerializedSize;

	public void Serialize(DateOnly value, Span<byte> buffer, INodeDataStore dataStore) => innerSerializer.Serialize(value.DayNumber, buffer, dataStore);

	public DateOnly Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore) => DateOnly.FromDayNumber(innerSerializer.Deserialize(buffer, dataStore));
}
