using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public class Int64LittleEndianSerializer : IPrimitiveSerializer<long>
{
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
	public void Serialize(long value, ref Span<byte> buffer) => BinaryPrimitives.WriteInt64LittleEndian(buffer, value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public long Deserialize(ReadOnlySpan<byte> buffer) => BinaryPrimitives.ReadInt64LittleEndian(buffer);
}
