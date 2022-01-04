using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Pando.Serialization.Utils;

namespace Pando.Serialization.PrimitiveSerializers;

public class Int16LittleEndianSerializer : IPrimitiveSerializer<short>
{
	public static Int16LittleEndianSerializer Default { get; } = new();

	private const int SIZE = sizeof(short);

	public int? ByteCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SIZE;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ByteCountForValue(short value) => SIZE;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(short value, ref Span<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		BinaryPrimitives.WriteInt16LittleEndian(slice, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public short Deserialize(ref ReadOnlySpan<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		return BinaryPrimitives.ReadInt16LittleEndian(slice);
	}
}
