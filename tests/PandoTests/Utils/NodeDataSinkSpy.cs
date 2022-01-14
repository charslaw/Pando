using System;
using System.Collections.Generic;
using Pando.DataSources;

namespace PandoTests.Utils;

/// Used to verify that a node data sink will get called with the correct values
public class NodeDataSinkSpy : INodeDataSink
{
	public List<byte[]> ReceivedNodeBytes { get; } = new();

	public ulong AddNode(ReadOnlySpan<byte> bytes)
	{
		ReceivedNodeBytes.Add(bytes.ToArray());

		return default;
	}
}
