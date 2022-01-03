using System;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public class TimeOnlyTicksSerializer : IPrimitiveSerializer<TimeOnly>
{
	public static TimeOnlyTicksSerializer Default { get; } = new();

	private readonly IPrimitiveSerializer<long> _innerSerializer;

	public TimeOnlyTicksSerializer() : this(Int64LittleEndianSerializer.Default) { }

	public TimeOnlyTicksSerializer(IPrimitiveSerializer<long> longSerializer)
	{
		_innerSerializer = longSerializer;
		ByteCount = _innerSerializer.ByteCount;
	}

	public int? ByteCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ByteCountForValue(TimeOnly value) => ByteCount ?? _innerSerializer.ByteCountForValue(value.Ticks);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(TimeOnly value, Span<byte> buffer) => _innerSerializer.Serialize(value.Ticks, buffer);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TimeOnly Deserialize(ReadOnlySpan<byte> buffer) => new(_innerSerializer.Deserialize(buffer));
}
