using System;
using Pando.Vaults;

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

	public void Serialize(TimeOnly value, Span<byte> buffer, INodeVault nodeVault) =>
		innerSerializer.Serialize(value.Ticks, buffer, nodeVault);

	public TimeOnly Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault) =>
		new(innerSerializer.Deserialize(buffer, nodeVault));
}
