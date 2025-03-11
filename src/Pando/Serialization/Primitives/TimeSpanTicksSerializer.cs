using System;
using Pando.Vaults;

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

	public void Serialize(TimeSpan value, Span<byte> buffer, INodeVault nodeVault) =>
		innerSerializer.Serialize(value.Ticks, buffer, nodeVault);

	public TimeSpan Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault) =>
		TimeSpan.FromTicks(innerSerializer.Deserialize(buffer, nodeVault));
}
