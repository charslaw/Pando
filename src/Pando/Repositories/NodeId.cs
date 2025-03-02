using System;
using System.Buffers.Binary;

namespace Pando.Repositories;

public readonly record struct NodeId(ulong Hash)
{
	public const int SIZE = sizeof(ulong);
	public static readonly NodeId None = new(0);

	public static NodeId FromBuffer(ReadOnlySpan<byte> buffer) => new(BinaryPrimitives.ReadUInt64LittleEndian(buffer));

	public void CopyTo(Span<byte> buffer) => BinaryPrimitives.WriteUInt64LittleEndian(buffer, Hash);

	public byte[] ToByteArray()
	{
		var bytes = new byte[SIZE];
		CopyTo(bytes);
		return bytes;
	}

	public override string ToString()
	{
		return $"NodeId( {Convert.ToHexStringLower(ToByteArray())} )";
	}
}
