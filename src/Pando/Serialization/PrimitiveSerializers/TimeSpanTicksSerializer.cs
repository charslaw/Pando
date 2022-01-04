using System;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

/// <summary>
/// Serializes/deserializes a <see cref="TimeSpan"/> via an <c>long</c> serializer
/// to serialize the <c>TimeSpan</c>'s <see cref="TimeSpan.Ticks"/>.
/// </summary>
public class TimeSpanTicksSerializer : IPrimitiveSerializer<TimeSpan>
{
	/// <summary>A global default instance for <see cref="TimeSpanTicksSerializer"/></summary>
	public static TimeSpanTicksSerializer Default { get; } = new();

	private readonly IPrimitiveSerializer<long> _innerSerializer;

	/// <summary>Create a <see cref="TimeSpanTicksSerializer"/> with the default
	/// <see cref="Int64LittleEndianSerializer"/> to serialize the Ticks.</summary>
	public TimeSpanTicksSerializer() : this(Int64LittleEndianSerializer.Default) { }

	/// <summary>Create a <see cref="TimeSpanTicksSerializer"/>
	/// with the given long serializer to serialize the Ticks.</summary>
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
