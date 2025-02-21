using System;
using System.Buffers.Binary;
using Pando.DataSources;

namespace Pando.Serialization.Primitives;

public class SingleLittleEndianSerializer : IPandoSerializer<float>
{
	/// <summary>A global default instance for <see cref="SingleLittleEndianSerializer"/></summary>
	public static SingleLittleEndianSerializer Default { get; } = new();

	public int SerializedSize => sizeof(float);

	public void Serialize(float value, Span<byte> buffer, INodeDataStore _) => BinaryPrimitives.WriteSingleLittleEndian(buffer, value);

	public float Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore _) => BinaryPrimitives.ReadSingleLittleEndian(buffer);
}
