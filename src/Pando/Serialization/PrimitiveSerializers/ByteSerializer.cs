using System;
using System.Runtime.CompilerServices;
using Pando.Serialization.Utils;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes/deserializes single <c>byte</c> values
public class ByteSerializer : IPrimitiveSerializer<byte>
{
	/// <summary>A global default instance for <see cref="ByteSerializer"/></summary>
	public static ByteSerializer Default { get; } = new();

	private const int SIZE = sizeof(byte);

	public int? ByteCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SIZE;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ByteCountForValue(byte value) => SIZE;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(byte value, ref Span<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		slice[0] = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte Deserialize(ref ReadOnlySpan<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		return slice[0];
	}
}