using System;
using System.Buffers.Binary;

namespace Pando.DataSources.Utils;

/// Utilities for converting to and from raw bytes (in little endian format)
internal static class ByteEncoder
{
	public static byte[] GetBytes(ulong value)
	{
		var arr = new byte[sizeof(ulong)];
		BinaryPrimitives.WriteUInt64LittleEndian(arr, value);
		return arr;
	}

	public static byte[] GetBytes(long value)
	{
		var arr = new byte[sizeof(long)];
		BinaryPrimitives.WriteInt64LittleEndian(arr, value);
		return arr;
	}

	public static byte[] GetBytes(int value)
	{
		var arr = new byte[sizeof(int)];
		BinaryPrimitives.WriteInt32LittleEndian(arr, value);
		return arr;
	}

	public static void CopyBytes(ulong value, Span<byte> destination) => BinaryPrimitives.WriteUInt64LittleEndian(destination, value);
	public static void CopyBytes(long value, Span<byte> destination) => BinaryPrimitives.WriteInt64LittleEndian(destination, value);
	public static void CopyBytes(int value, Span<byte> destination) => BinaryPrimitives.WriteInt32LittleEndian(destination, value);

	public static ulong GetUInt64(ReadOnlySpan<byte> bytes) => BinaryPrimitives.ReadUInt64LittleEndian(bytes);
	public static long GetInt64(ReadOnlySpan<byte> bytes) => BinaryPrimitives.ReadInt64LittleEndian(bytes);
	public static int GetInt32(ReadOnlySpan<byte> bytes) => BinaryPrimitives.ReadInt32LittleEndian(bytes);
}
