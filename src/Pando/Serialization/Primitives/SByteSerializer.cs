using System;
using Pando.DataSources;

namespace Pando.Serialization.Primitives;

/// Serializes/deserializes <c>sbyte</c> values
public class SByteSerializer : IPandoSerializer<sbyte>
{
	/// <summary>A global default instance for <see cref="SByteSerializer"/></summary>
	public static SByteSerializer Default { get; } = new();

	public int SerializedSize => sizeof(sbyte);

	public void Serialize(sbyte value, Span<byte> buffer, INodeDataStore dataStore)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(buffer.Length, nameof(buffer));
		buffer[0] = (byte)value;
	}

	public sbyte Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(buffer.Length, nameof(buffer));
		return (sbyte)buffer[0];
	}
}
