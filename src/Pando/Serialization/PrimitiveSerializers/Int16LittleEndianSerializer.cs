using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public class Int16LittleEndianSerializer : IPrimitiveSerializer<short>
{
	public static Int16LittleEndianSerializer Default { get; } = new();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(short value, ref Span<byte> buffer) => BinaryPrimitives.WriteInt16LittleEndian(buffer, value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public short Deserialize(ReadOnlySpan<byte> buffer) => BinaryPrimitives.ReadInt16LittleEndian(buffer);
}
