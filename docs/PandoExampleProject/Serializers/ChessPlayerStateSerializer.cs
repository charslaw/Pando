using System;
using Pando.DataSources;
using Pando.Serialization;

namespace PandoExampleProject.Serializers;

internal class ChessPlayerStateSerializer : INodeSerializer<ChessPlayerState>
{
	private const int SIZE = 2;
	public int? NodeSize => SIZE;

	public ulong Serialize(ChessPlayerState obj, INodeDataSink dataSink)
	{
		Span<byte> buffer = stackalloc byte[SIZE];

		buffer[0] = (byte)obj.CurrentTurn;
		buffer[1] = (byte)obj.Winner;

		return dataSink.AddNode(buffer);
	}

	public ChessPlayerState Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource)
	{
		return new ChessPlayerState((Player)bytes[0], (Winner)bytes[1]);
	}
}
