using System;
using Pando.DataSources;
using Pando.Serialization;

namespace PandoTests.Utils;

/// An implementation of INodeReader that just converts the span to an array. This is used only for testing, to inspect the raw bytes.
internal readonly struct ToArrayReader : INodeReader<byte[]>
{
	public byte[] Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource) => bytes.ToArray();
}
