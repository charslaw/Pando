using System;
using System.Buffers.Binary;
using Pando.DataSources;
using Pando.Serialization.PrimitiveSerializers;

namespace Pando.Serialization.Primitives;

public class HalfLittleEndianSerializer : IPandoSerializer<Half>
{
	/// <summary>A global default instance for <see cref="SingleLittleEndianSerializer"/></summary>
	public static HalfLittleEndianSerializer Default { get; } = new();

	public unsafe int SerializedSize => sizeof(Half);

	public void Serialize(Half value, Span<byte> buffer, INodeDataSink _) => BinaryPrimitives.WriteHalfLittleEndian(buffer, value);

	public Half Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource _) => BinaryPrimitives.ReadHalfLittleEndian(buffer);
}
