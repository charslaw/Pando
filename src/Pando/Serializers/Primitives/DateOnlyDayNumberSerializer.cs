using System;
using Pando.Vaults;

namespace Pando.Serializers.Primitives;

/// <summary>
/// Serializes/deserializes a <see cref="DateOnly"/> via an <c>int</c> serializer
/// to serialize the <c>DateOnly</c>'s <see cref="DateOnly.DayNumber"/>.
/// </summary>
public class DateOnlyDayNumberSerializer(IPandoSerializer<int> innerSerializer) : IPandoSerializer<DateOnly>
{
	/// <summary>A global default instance for <see cref="DateOnlyDayNumberSerializer"/></summary>
	public static DateOnlyDayNumberSerializer Default { get; } = new(Int32LittleEndianSerializer.Default);

	public int SerializedSize { get; } = innerSerializer.SerializedSize;

	public void Serialize(DateOnly value, Span<byte> buffer, INodeVault nodeVault) =>
		innerSerializer.Serialize(value.DayNumber, buffer, nodeVault);

	public DateOnly Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault) =>
		DateOnly.FromDayNumber(innerSerializer.Deserialize(buffer, nodeVault));
}
