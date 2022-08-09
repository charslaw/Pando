using System;

namespace SerializerGeneratorIntegrationTests.Fakes;

public class SequentialHashNodeSink : SpyNodeSink
{
	private ulong _currentHash;
	public SequentialHashNodeSink(ulong startingHash) { _currentHash = startingHash; }

	public override ulong AddNode(ReadOnlySpan<byte> bytes)
	{
		base.AddNode(bytes);
		return _currentHash++;
	}
}
