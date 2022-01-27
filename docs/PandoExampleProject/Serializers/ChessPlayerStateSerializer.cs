using System;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers;

namespace PandoExampleProject.Serializers;

internal class ChessPlayerStateSerializer : INodeSerializer<ChessPlayerState>
{
	private const int SIZE = 2 * sizeof(byte);
	public int? NodeSize => SIZE;

	public int NodeSizeForObject(ChessPlayerState obj) => SIZE;

	public void Serialize(ChessPlayerState obj, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		writeBuffer[0] = (byte)obj.CurrentTurn;
		writeBuffer[1] = (byte)obj.Winner;
	}

	public ChessPlayerState Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		return new ChessPlayerState((Player)readBuffer[0], (Winner)readBuffer[1]);
	}
}
