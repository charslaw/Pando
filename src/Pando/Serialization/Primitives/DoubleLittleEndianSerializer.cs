using System;
using System.Buffers.Binary;
using Pando.DataSources;
using Pando.Serialization.PrimitiveSerializers;

namespace Pando.Serialization.Primitives;

public class DoubleLittleEndianSerializer : IPandoSerializer<double>
{
	/// <summary>A global default instance for <see cref="SingleLittleEndianSerializer"/></summary>
	public static DoubleLittleEndianSerializer Default { get; } = new();

	public int SerializedSize => sizeof(double);

	public void Serialize(double value, Span<byte> slice, INodeDataSink _) => BinaryPrimitives.WriteDoubleLittleEndian(slice, value);

	public double Deserialize(ReadOnlySpan<byte> slice, INodeDataSource _) => BinaryPrimitives.ReadDoubleLittleEndian(slice);
}
