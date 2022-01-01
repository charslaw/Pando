using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public class Int64LittleEndianSerializer : IPrimitiveSerializer<long>
{
	public static Int64LittleEndianSerializer Default { get; } = new();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(long value, ref Span<byte> buffer) => BinaryPrimitives.WriteInt64LittleEndian(buffer, value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public long Deserialize(ReadOnlySpan<byte> buffer) => BinaryPrimitives.ReadInt64LittleEndian(buffer);
}
