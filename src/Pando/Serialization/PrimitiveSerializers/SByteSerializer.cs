using System;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public class SByteSerializer : IPrimitiveSerializer<sbyte>
{
	public static SByteSerializer Default { get; } = new();

	private const int SIZE = sizeof(sbyte);

	public int? ByteCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SIZE;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ByteCountForValue(sbyte value) => SIZE;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(sbyte value, ref Span<byte> buffer) { buffer[0] = (byte)value; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sbyte Deserialize(ReadOnlySpan<byte> buffer) => (sbyte)buffer[0];
}
