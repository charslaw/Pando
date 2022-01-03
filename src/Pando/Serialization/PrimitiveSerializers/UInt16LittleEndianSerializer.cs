using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public class UInt16LittleEndianSerializer : IPrimitiveSerializer<ushort>
{
	public static UInt16LittleEndianSerializer Default { get; } = new();

	private const int SIZE = sizeof(ushort);

	public int? ByteCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SIZE;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ByteCountForValue(ushort value) => SIZE;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(ushort value, ref Span<byte> buffer) => BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ushort Deserialize(ReadOnlySpan<byte> buffer) => BinaryPrimitives.ReadUInt16LittleEndian(buffer);
}
