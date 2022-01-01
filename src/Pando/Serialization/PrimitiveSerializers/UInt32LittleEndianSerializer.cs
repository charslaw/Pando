using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public class UInt32LittleEndianSerializer : IPrimitiveSerializer<uint>
{
	public static UInt32LittleEndianSerializer Default { get; } = new();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(uint value, ref Span<byte> buffer) => BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public uint Deserialize(ReadOnlySpan<byte> buffer) => BinaryPrimitives.ReadUInt32LittleEndian(buffer);
}
