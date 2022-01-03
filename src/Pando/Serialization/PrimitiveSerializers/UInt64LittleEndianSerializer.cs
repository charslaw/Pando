using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public class UInt64LittleEndianSerializer : IPrimitiveSerializer<ulong>
{
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
	public void Serialize(ulong value, Span<byte> buffer) => BinaryPrimitives.WriteUInt64LittleEndian(buffer, value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ulong Deserialize(ReadOnlySpan<byte> buffer) => BinaryPrimitives.ReadUInt64LittleEndian(buffer);
}
