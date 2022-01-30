using System;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers;

namespace PandoExampleProject.Serializers;

/// ChessPlayerState only contains primitive data, so this serializer is pretty simple.
internal class ChessPlayerStateSerializer : INodeSerializer<ChessPlayerState>
{
	private const int SIZE = 2 * sizeof(byte);
	public int? NodeSize => SIZE;

	public int NodeSizeForObject(ChessPlayerState obj) => SIZE;

	/// Sequentially writes each enum member of the chess player state into a buffer and submits the node to the data sink.
	public void Serialize(ChessPlayerState obj, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		writeBuffer[0] = (byte)obj.CurrentTurn;
		writeBuffer[1] = (byte)obj.Winner;
	}

	/// Gets the sequential byte values and converts them to their enum values, then creates a chess player state.
	public ChessPlayerState Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		return new ChessPlayerState((Player)readBuffer[0], (Winner)readBuffer[1]);
	}
}
