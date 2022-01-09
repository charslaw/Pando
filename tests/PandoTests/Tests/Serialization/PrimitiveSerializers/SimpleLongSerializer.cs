using System;
using System.Buffers.Binary;
using Pando.Serialization.PrimitiveSerializers;

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

/// A simply implemented long serializer that serializes in big endian encoding
/// <remarks>This exists to provide a very simple, self-contained serializer that upholds the
/// IPrimitiveSerializer contract in as few lines as possible for testing.</remarks>
internal class SimpleLongSerializer : IPrimitiveSerializer<long>
{
	public int? ByteCount => sizeof(long);
	public int ByteCountForValue(long value) => sizeof(long);

	public void Serialize(long value, ref Span<byte> buffer)
	{
		BinaryPrimitives.WriteInt64BigEndian(buffer, value);
		buffer = buffer[sizeof(ulong)..];
	}

	public long Deserialize(ref ReadOnlySpan<byte> buffer)
	{
		var result = BinaryPrimitives.ReadInt64BigEndian(buffer);
		buffer = buffer[sizeof(ulong)..];
		return result;
	}
}
