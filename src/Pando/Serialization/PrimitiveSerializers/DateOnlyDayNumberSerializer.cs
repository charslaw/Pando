using System;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public class DateOnlyDayNumberSerializer : IPrimitiveSerializer<DateOnly>
{
	public static DateOnlyDayNumberSerializer Default { get; } = new();

	private readonly IPrimitiveSerializer<int> _innerSerializer;

	public DateOnlyDayNumberSerializer() : this(Int32LittleEndianSerializer.Default) { }

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
	public void Serialize(DateOnly value, Span<byte> buffer) => _innerSerializer.Serialize(value.DayNumber, buffer);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DateOnly Deserialize(ReadOnlySpan<byte> buffer) => DateOnly.FromDayNumber(_innerSerializer.Deserialize(buffer));
}
