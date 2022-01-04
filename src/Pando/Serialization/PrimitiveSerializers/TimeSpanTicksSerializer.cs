using System;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public class TimeSpanTicksSerializer : IPrimitiveSerializer<TimeSpan>
{
	public static TimeSpanTicksSerializer Default { get; } = new();

	private readonly IPrimitiveSerializer<long> _innerSerializer;

	public TimeSpanTicksSerializer() : this(Int64LittleEndianSerializer.Default) { }

	public TimeSpanTicksSerializer(IPrimitiveSerializer<long> longSerializer)
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
	public int ByteCountForValue(TimeSpan value) => ByteCount ?? _innerSerializer.ByteCountForValue(value.Ticks);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(TimeSpan value, ref Span<byte> buffer) => _innerSerializer.Serialize(value.Ticks, ref buffer);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TimeSpan Deserialize(ref ReadOnlySpan<byte> buffer) => TimeSpan.FromTicks(_innerSerializer.Deserialize(ref buffer));
}
