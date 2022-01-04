using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Pando.Serialization.Utils;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes/deserializes <c>long</c> values in little endian encoding (least significant byte first)
public class Int64LittleEndianSerializer : IPrimitiveSerializer<long>
{
	/// <summary>A global default instance for <see cref="Int64LittleEndianSerializer"/></summary>
	public static Int64LittleEndianSerializer Default { get; } = new();

	private const int SIZE = sizeof(long);

	public int? ByteCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SIZE;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ByteCountForValue(long value) => SIZE;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(long value, ref Span<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		BinaryPrimitives.WriteInt64LittleEndian(slice, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public long Deserialize(ref ReadOnlySpan<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		return BinaryPrimitives.ReadInt64LittleEndian(slice);
	}
}
