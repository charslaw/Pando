using System;
using Pando.DataSources;
using Pando.Serialization;

namespace PandoExampleProject.Serializers;

internal class ChessPlayerStateSerializer : INodeSerializer<ChessPlayerState>
{
	public ulong Serialize(ChessPlayerState obj, INodeDataSink dataSink)
	{
		Span<byte> buffer = stackalloc byte[2];

		buffer[0] = (byte)obj.CurrentTurn;
		buffer[1] = (byte)obj.Winner;

		return dataSink.AddNode(buffer);
	}

	public ChessPlayerState Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource)
	{
		return new ChessPlayerState((Player)bytes[0], (Winner)bytes[1]);
	}
}
