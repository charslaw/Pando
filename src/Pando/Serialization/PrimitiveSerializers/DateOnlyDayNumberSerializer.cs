using System;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

/// <summary>
/// Serializes/deserializes a <see cref="DateOnly"/> via an <c>int</c> serializer
/// to serialize the <c>DateOnly</c>'s <see cref="DateOnly.DayNumber"/>.
/// </summary>
public class DateOnlyDayNumberSerializer : IPrimitiveSerializer<DateOnly>
{
	/// <summary>A global default instance for <see cref="DateOnlyDayNumberSerializer"/></summary>
	public static DateOnlyDayNumberSerializer Default { get; } = new();

	private readonly IPrimitiveSerializer<int> _innerSerializer;

	/// <summary>Create a <see cref="DateOnlyDayNumberSerializer"/> with the default
	/// <see cref="Int32LittleEndianSerializer"/> to serialize the DayNumber.</summary>
	public DateOnlyDayNumberSerializer() : this(Int32LittleEndianSerializer.Default) { }

	/// <summary>Create a <see cref="DateOnlyDayNumberSerializer"/>
	/// with the given int serializer to serialize the DayNumber.</summary>
	public DateOnlyDayNumberSerializer(IPrimitiveSerializer<int> intSerializer)
	{
		_innerSerializer = intSerializer;
		ByteCount = _innerSerializer.ByteCount;
	}

	public int? ByteCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ByteCountForValue(DateOnly value) => ByteCount ?? _innerSerializer.ByteCountForValue(value.DayNumber);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(DateOnly value, ref Span<byte> buffer) => _innerSerializer.Serialize(value.DayNumber, ref buffer);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DateOnly Deserialize(ref ReadOnlySpan<byte> buffer) => DateOnly.FromDayNumber(_innerSerializer.Deserialize(ref buffer));
}
