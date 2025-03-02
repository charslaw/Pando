using System;
using Pando.DataSources;

namespace Pando.Serialization.Primitives;

/// <summary> Serializes/deserializes <see cref="Nullable{T}"/> values via a provided inner serializer.</summary>
public class NullableSerializer<T>(IPandoSerializer<T> innerSerializer) : IPandoSerializer<T?>
	where T : struct
{
	public int SerializedSize { get; } = 1 + innerSerializer.SerializedSize;

	public void Serialize(T? value, Span<byte> buffer, INodeDataStore dataStore)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(buffer.Length, SerializedSize, nameof(buffer));

		if (value is not null)
		{
			buffer[0] = 1;
			innerSerializer.Serialize(value.Value, buffer[1..], dataStore);
		}
		else
		{
			buffer.Clear();
		}
	}

	public T? Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(buffer.Length, SerializedSize, nameof(buffer));
		return buffer[0] == 0 ? null : innerSerializer.Deserialize(buffer[1..], dataStore);
	}
}
