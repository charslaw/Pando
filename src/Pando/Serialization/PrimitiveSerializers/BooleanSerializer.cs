using System;
using System.Runtime.CompilerServices;

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
		buffer[0] = (byte)(value ? 1 : 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Deserialize(ReadOnlySpan<byte> buffer) => buffer[0] != 0;
}
