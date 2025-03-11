using System;
using Pando.Vaults;

namespace Pando.Serialization.Primitives;

/// Serializes/deserializes <c>bool</c> values as a single byte
public class BooleanSerializer : IPandoSerializer<bool>
{
	/// <summary>A global default instance for <see cref="BooleanSerializer"/></summary>
	public static BooleanSerializer Default { get; } = new();

	public int SerializedSize => sizeof(byte);

	public void Serialize(bool value, Span<byte> buffer, INodeVault nodeVault)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(buffer.Length, nameof(buffer));
		buffer[0] = (byte)(value ? 1 : 0);
	}

	public bool Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(buffer.Length, nameof(buffer));
		return buffer[0] != 0;
	}
}
