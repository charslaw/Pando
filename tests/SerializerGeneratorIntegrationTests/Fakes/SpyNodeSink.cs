using System;
using Pando.DataSources;

namespace SerializerGeneratorIntegrationTests.Fakes;

public class SpyNodeSink : INodeDataSink
{
	public int AddNodeCallCount { get; private set; }

	public ulong AddNode(ReadOnlySpan<byte> bytes)
	{
		AddNodeCallCount++;
		return default;
	}
}
