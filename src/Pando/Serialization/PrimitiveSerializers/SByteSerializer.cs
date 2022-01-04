using System;
using System.Runtime.CompilerServices;
using Pando.Serialization.Utils;

namespace Pando.Serialization.PrimitiveSerializers;

/// Serializes/deserializes <c>sbyte</c> values
public class SByteSerializer : IPrimitiveSerializer<sbyte>
{
	/// <summary>A global default instance for <see cref="SByteSerializer"/></summary>
	public static SByteSerializer Default { get; } = new();

	private const int SIZE = sizeof(sbyte);

	public int? ByteCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SIZE;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ByteCountForValue(sbyte value) => SIZE;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(sbyte value, ref Span<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		slice[0] = (byte)value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sbyte Deserialize(ref ReadOnlySpan<byte> buffer)
	{
		var slice = SpanHelpers.PopStart(ref buffer, SIZE);
		return (sbyte)slice[0];
	}
}
