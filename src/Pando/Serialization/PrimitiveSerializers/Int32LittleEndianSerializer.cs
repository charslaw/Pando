using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Pando.Serialization.Utils;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes/deserializes <c>int</c> values in little endian encoding (least significant byte first)
public class Int32LittleEndianSerializer : IPrimitiveSerializer<int>
{
	/// <summary>A global default instance for <see cref="Int32LittleEndianSerializer"/></summary>
	public static Int32LittleEndianSerializer Default { get; } = new();

	private const int SIZE = sizeof(int);

	public int? ByteCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SIZE;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ByteCountForValue(int value) => SIZE;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(int value, ref Span<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		BinaryPrimitives.WriteInt32LittleEndian(slice, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int Deserialize(ref ReadOnlySpan<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		return BinaryPrimitives.ReadInt32LittleEndian(slice);
	}
}