using System;
using Pando.DataSources;

namespace Pando.Serialization.Primitives;

/// Serializes/deserializes single <c>byte</c> values
public class ByteSerializer : IPandoSerializer<byte>
{
	/// <summary>A global default instance for <see cref="ByteSerializer"/></summary>
	public static ByteSerializer Default { get; } = new();

	public int SerializedSize => sizeof(byte);

	public void Serialize(byte value, Span<byte> buffer, INodeDataSink _)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(buffer.Length, nameof(buffer));
		buffer[0] = value;
	}

	public byte Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource _)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(buffer.Length, nameof(buffer));
		return buffer[0];
	}
}
