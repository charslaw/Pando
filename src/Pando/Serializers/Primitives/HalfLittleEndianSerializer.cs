using System;
using System.Buffers.Binary;
using Pando.Vaults;

namespace Pando.Serializers.Primitives;

public class HalfLittleEndianSerializer : IPandoSerializer<Half>
{
	/// <summary>A global default instance for <see cref="SingleLittleEndianSerializer"/></summary>
	public static HalfLittleEndianSerializer Default { get; } = new();

	public int SerializedSize => sizeof(short);

	public void Serialize(Half value, Span<byte> buffer, INodeVault nodeVault) =>
		BinaryPrimitives.WriteHalfLittleEndian(buffer, value);

	public Half Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault) =>
		BinaryPrimitives.ReadHalfLittleEndian(buffer);
}
