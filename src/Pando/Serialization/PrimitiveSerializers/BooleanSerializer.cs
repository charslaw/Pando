using System;
using System.Runtime.CompilerServices;
using Pando.Serialization.Utils;

namespace Pando.Serialization.PrimitiveSerializers;

public class BooleanSerializer : IPrimitiveSerializer<bool>
{
	public static BooleanSerializer Default { get; } = new();

	private const int SIZE = 1;

	public int? ByteCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SIZE;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ByteCountForValue(bool value) => SIZE;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(bool value, ref Span<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		slice[0] = (byte)(value ? 1 : 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Deserialize(ref ReadOnlySpan<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		return slice[0] != 0;
	}
}
