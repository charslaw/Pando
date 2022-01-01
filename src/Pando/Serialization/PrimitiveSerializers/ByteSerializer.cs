using System;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public class ByteSerializer : IPrimitiveSerializer<byte>
{
	public static ByteSerializer Default { get; } = new();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(byte value, ref Span<byte> buffer) { buffer[0] = value; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte Deserialize(ReadOnlySpan<byte> buffer) => buffer[0];
}
