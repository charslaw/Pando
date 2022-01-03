using System;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public class ByteSerializer : IPrimitiveSerializer<byte>
{
	public static ByteSerializer Default { get; } = new();

	private const int SIZE = sizeof(byte);

	public int? ByteCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SIZE;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ByteCountForValue(byte value) => SIZE;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(byte value, Span<byte> buffer) { buffer[0] = value; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte Deserialize(ReadOnlySpan<byte> buffer) => buffer[0];
}
