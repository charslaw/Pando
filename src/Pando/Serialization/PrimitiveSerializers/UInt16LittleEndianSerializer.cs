using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public class UInt16LittleEndianSerializer : IPrimitiveSerializer<ushort>
{
	public static UInt16LittleEndianSerializer Default { get; } = new();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(ushort value, ref Span<byte> buffer) => BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ushort Deserialize(ReadOnlySpan<byte> buffer) => BinaryPrimitives.ReadUInt16LittleEndian(buffer);
}
