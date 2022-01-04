using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Pando.Serialization.Utils;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes/deserializes <c>ulong</c> values in little endian encoding (least significant byte first)
public class UInt64LittleEndianSerializer : IPrimitiveSerializer<ulong>
{
	/// <summary>A global default instance for <see cref="UInt64LittleEndianSerializer"/></summary>
	public static UInt64LittleEndianSerializer Default { get; } = new();

	private const int SIZE = sizeof(ulong);

	public int? ByteCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SIZE;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ByteCountForValue(ulong value) => SIZE;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(ulong value, ref Span<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		BinaryPrimitives.WriteUInt64LittleEndian(slice, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ulong Deserialize(ref ReadOnlySpan<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		return BinaryPrimitives.ReadUInt64LittleEndian(slice);
	}
}
