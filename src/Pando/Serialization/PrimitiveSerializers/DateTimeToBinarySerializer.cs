using System;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

/// <summary>
/// Serializes/deserializes a <see cref="DateTime"/> via a <c>long</c> serializer
/// to serialize the <c>DateTime</c>'s <see cref="DateTime.ToBinary"/> encoding.
/// </summary>
public class DateTimeToBinarySerializer : IPrimitiveSerializer<DateTime>
{
	/// <summary>A global default instance for <see cref="DateTimeToBinarySerializer"/></summary>
	public static DateTimeToBinarySerializer Default { get; } = new();

	private readonly IPrimitiveSerializer<long> _innerSerializer;

	/// <summary>Create a <see cref="DateTimeToBinarySerializer"/> with the default
	/// <see cref="Int64LittleEndianSerializer"/> to serialize the ToBinary encoding of the DateTime.</summary>
	public DateTimeToBinarySerializer() : this(Int64LittleEndianSerializer.Default) { }

	/// <summary>Create a <see cref="DateTimeToBinarySerializer"/> with the given long serializer
	/// to serialize the ToBinary encoding of the DateTime.</summary>
	public DateTimeToBinarySerializer(IPrimitiveSerializer<long> longSerializer)
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
	public void Serialize(DateTime value, ref Span<byte> buffer) => _innerSerializer.Serialize(value.ToBinary(), ref buffer);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DateTime Deserialize(ref ReadOnlySpan<byte> buffer) => DateTime.FromBinary(_innerSerializer.Deserialize(ref buffer));
}
