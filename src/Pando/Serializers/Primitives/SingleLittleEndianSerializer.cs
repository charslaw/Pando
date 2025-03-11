using System;
using System.Buffers.Binary;
using Pando.Vaults;

namespace Pando.Serializers.Primitives;

public class SingleLittleEndianSerializer : IPandoSerializer<float>
{
	/// <summary>A global default instance for <see cref="SingleLittleEndianSerializer"/></summary>
	public static SingleLittleEndianSerializer Default { get; } = new();

	public int SerializedSize => sizeof(float);

	public void Serialize(float value, Span<byte> buffer, INodeVault nodeVault) =>
		BinaryPrimitives.WriteSingleLittleEndian(buffer, value);

	public float Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault) =>
		BinaryPrimitives.ReadSingleLittleEndian(buffer);
}
