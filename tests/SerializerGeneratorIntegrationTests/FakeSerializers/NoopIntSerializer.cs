using System;
using Pando.Serialization.PrimitiveSerializers;

namespace SerializerGeneratorIntegrationTests.FakeSerializers;

internal class NoopIntSerializer : IPrimitiveSerializer<int>
{
	public int? ByteCount => null;
	public int ByteCountForValue(int value) => throw new NotImplementedException();
	public void Serialize(int value, ref Span<byte> buffer) => throw new NotImplementedException();
	public int Deserialize(ref ReadOnlySpan<byte> buffer) => throw new NotImplementedException();
}
