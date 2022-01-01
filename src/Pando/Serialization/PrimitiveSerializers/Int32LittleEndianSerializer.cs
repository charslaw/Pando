using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public class Int32LittleEndianSerializer : IPrimitiveSerializer<int>
{
	public static Int32LittleEndianSerializer Default { get; } = new();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(int value, ref Span<byte> buffer) => BinaryPrimitives.WriteInt32LittleEndian(buffer, value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int Deserialize(ReadOnlySpan<byte> buffer) => BinaryPrimitives.ReadInt32LittleEndian(buffer);
}
