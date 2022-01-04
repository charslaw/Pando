using System;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

/// <summary>
/// Serializes/deserializes a <see cref="TimeOnly"/> via an <c>long</c> serializer
/// to serialize the <c>TimeOnly</c>'s <see cref="TimeOnly.Ticks"/>.
/// </summary>
public class TimeOnlyTicksSerializer : IPrimitiveSerializer<TimeOnly>
{
	/// <summary>A global default instance for <see cref="TimeOnlyTicksSerializer"/></summary>
	public static TimeOnlyTicksSerializer Default { get; } = new();

	private readonly IPrimitiveSerializer<long> _innerSerializer;

	/// <summary>Create a <see cref="TimeOnlyTicksSerializer"/> with the default
	/// <see cref="Int64LittleEndianSerializer"/> to serialize the Ticks.</summary>
	public TimeOnlyTicksSerializer() : this(Int64LittleEndianSerializer.Default) { }

	/// <summary>Create a <see cref="TimeOnlyTicksSerializer"/>
	/// with the given long serializer to serialize the Ticks.</summary>
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
	public void Serialize(TimeOnly value, ref Span<byte> buffer) => _innerSerializer.Serialize(value.Ticks, ref buffer);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TimeOnly Deserialize(ref ReadOnlySpan<byte> buffer) => new(_innerSerializer.Deserialize(ref buffer));
}
