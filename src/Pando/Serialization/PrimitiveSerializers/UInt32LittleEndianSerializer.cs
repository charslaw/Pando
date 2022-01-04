using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Pando.Serialization.Utils;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes/deserializes <c>uint</c> values in little endian encoding (least significant byte first)
public class UInt32LittleEndianSerializer : IPrimitiveSerializer<uint>
{
	/// <summary>A global default instance for <see cref="UInt32LittleEndianSerializer"/></summary>
	public static UInt32LittleEndianSerializer Default { get; } = new();

	private const int SIZE = sizeof(uint);

	public int? ByteCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SIZE;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ByteCountForValue(uint value) => SIZE;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(uint value, ref Span<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		BinaryPrimitives.WriteUInt32LittleEndian(slice, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public uint Deserialize(ref ReadOnlySpan<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		return BinaryPrimitives.ReadUInt32LittleEndian(slice);
	}
}
