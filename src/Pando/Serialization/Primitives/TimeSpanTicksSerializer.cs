using System;
using Pando.DataSources;

namespace Pando.Serialization.Primitives;

/// <summary>
/// Serializes/deserializes a <see cref="TimeSpan"/> via an <c>long</c> serializer
/// to serialize the <c>TimeSpan</c>'s <see cref="TimeSpan.Ticks"/>.
/// </summary>
public class TimeSpanTicksSerializer(IPandoSerializer<long> innerSerializer) : IPandoSerializer<TimeSpan>
{
	/// <summary>A global default instance for <see cref="TimeSpanTicksSerializer"/></summary>
	public static TimeSpanTicksSerializer Default { get; } = new(Int64LittleEndianSerializer.Default);

	public int SerializedSize { get; } = innerSerializer.SerializedSize;

	public void Serialize(TimeSpan value, Span<byte> buffer, INodeDataSink dataSink) => innerSerializer.Serialize(value.Ticks, buffer, dataSink);

	public TimeSpan Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource) => TimeSpan.FromTicks(innerSerializer.Deserialize(buffer, dataSource));
}
