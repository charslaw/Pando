using System;
using Pando.Serialization.PrimitiveSerializers;

namespace SerializerGeneratorIntegrationTests.FakeSerializers;

public class FixedSizePrimitiveSerializer<T> : IPrimitiveSerializer<T>
{
	public int? ByteCount { get; }
	public int ByteCountForValue(T value) => ByteCount!.Value;
	public void Serialize(T value, ref Span<byte> buffer) => throw new NotImplementedException();
	public T Deserialize(ref ReadOnlySpan<byte> buffer) => throw new NotImplementedException();

	public FixedSizePrimitiveSerializer(int size)
	{
		ByteCount = size;
	}
}
