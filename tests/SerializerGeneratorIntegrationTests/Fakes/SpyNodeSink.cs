using System;
using Pando.DataSources;

namespace SerializerGeneratorIntegrationTests.Fakes;

public class SpyNodeSink : INodeDataSink
{
	public int AddNodeCallCount { get; private set; }

	public virtual ulong AddNode(ReadOnlySpan<byte> bytes)
	{
		AddNodeCallCount++;
		return default;
	}
}
