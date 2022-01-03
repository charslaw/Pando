using System;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public class DateTimeUnixSerializer : IPrimitiveSerializer<DateTime>
{
	public static DateTimeUnixSerializer Default { get; } = new();

	private readonly IPrimitiveSerializer<long> _innerSerializer;

	public DateTimeUnixSerializer() : this(Int64LittleEndianSerializer.Default) { }

	public DateTimeUnixSerializer(IPrimitiveSerializer<long> longSerializer)
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
	public int ByteCountForValue(DateTime value) => ByteCount ?? _innerSerializer.ByteCountForValue(value.ToBinary());

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(DateTime value, Span<byte> buffer) => _innerSerializer.Serialize(value.ToBinary(), buffer);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DateTime Deserialize(ReadOnlySpan<byte> buffer) => DateTime.FromBinary(_innerSerializer.Deserialize(buffer));
}
