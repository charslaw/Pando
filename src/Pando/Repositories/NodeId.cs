using System;
using System.Buffers;
using System.Buffers.Binary;

namespace Pando.Repositories;

public readonly record struct NodeId(ulong Hash)
{
	public const int SIZE = sizeof(ulong);
	public static readonly NodeId None = new(0);

	public static NodeId FromBuffer(ReadOnlySpan<byte> buffer) => new(BinaryPrimitives.ReadUInt64LittleEndian(buffer));

	public static NodeId FromHashString(ReadOnlySpan<char> hashString)
	{
		Span<byte> buffer = stackalloc byte[SIZE];

		if (Convert.FromHexString(hashString, buffer, out _, out _) != OperationStatus.Done)
		{
			throw new ArgumentException($"'{hashString}' is not a valid NodeId hash string", nameof(hashString));
		}

		return FromBuffer(buffer);
	}

	public void CopyTo(Span<byte> buffer) => BinaryPrimitives.WriteUInt64LittleEndian(buffer, Hash);

	public byte[] ToByteArray()
	{
		var bytes = new byte[SIZE];
		CopyTo(bytes);
		return bytes;
	}

	public override string ToString()
	{
		Span<char> buffer = stackalloc char[SIZE * 2]; // hex representation is 2 chars per byte
		CopyHashStringTo(ref buffer);
		return $"NodeId( {buffer} )";
	}

	public string ToHashString()
	{
		Span<byte> buffer = stackalloc byte[SIZE];
		CopyTo(buffer);
		return Convert.ToHexStringLower(buffer);
	}

	public void CopyHashStringTo(ref Span<char> stringBuffer)
	{
		Span<byte> buffer = stackalloc byte[SIZE];
		CopyTo(buffer);
		if (Convert.TryToHexStringLower(buffer, stringBuffer, out var charsWritten))
		{
			stringBuffer = stringBuffer[..charsWritten];
		}
		else
		{
			throw new ArgumentOutOfRangeException(
				nameof(stringBuffer),
				"The provided string buffer was not large enough to copy the hash string."
			);
		}
	}
}
