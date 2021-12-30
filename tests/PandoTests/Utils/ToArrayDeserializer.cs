using System;
using Pando;
using Pando.DataSources;
using Pando.DataSources.Utils;

namespace PandoTests.Utils;

/// An implementation of IPandoNodeDeserializer that just converts the span to an array. This is used only for testing, to inspect the raw bytes.
internal readonly struct ToArrayDeserializer : IPandoNodeDeserializer<byte[]>
{
	public byte[] Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource) => bytes.ToArray();
}

/// An implementation of ISpanVisitor that just converts the span to an array. This is used only for testing, to inspect the raw bytes.
internal readonly struct ToArrayVisitor : ISpanVisitor<byte, byte[]>
{
	public byte[] Visit(ReadOnlySpan<byte> span) => span.ToArray();
}
