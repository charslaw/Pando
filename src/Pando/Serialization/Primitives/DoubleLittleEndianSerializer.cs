using System;
using System.Buffers.Binary;
using Pando.Vaults;

namespace Pando.Serialization.Primitives;

public class DoubleLittleEndianSerializer : IPandoSerializer<double>
{
	/// <summary>A global default instance for <see cref="SingleLittleEndianSerializer"/></summary>
	public static DoubleLittleEndianSerializer Default { get; } = new();

	public int SerializedSize => sizeof(double);

	public void Serialize(double value, Span<byte> buffer, INodeVault nodeVault) =>
		BinaryPrimitives.WriteDoubleLittleEndian(buffer, value);

	public double Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault) =>
		BinaryPrimitives.ReadDoubleLittleEndian(buffer);
}
