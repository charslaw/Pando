using System;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.PrimitiveSerializers;

public abstract class FixedSizeBaseSerializer<T> : IPrimitiveSerializer<T>
{
	public abstract int FixedSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
	}

	public int? ByteCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => FixedSize;
	}

	public int ByteCountForValue(T value) => FixedSize;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Serialize(T value, ref Span<byte> buffer)
	{
		var slice = buffer[..FixedSize];
		buffer = buffer[FixedSize..];
		SerializeInner(value, slice);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T Deserialize(ref ReadOnlySpan<byte> buffer)
	{
		var slice = buffer[..FixedSize];
		buffer = buffer[FixedSize..];
		return DeserializeInner(slice);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected abstract void SerializeInner(T value, Span<byte> slice);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected abstract T DeserializeInner(ReadOnlySpan<byte> slice);
}
