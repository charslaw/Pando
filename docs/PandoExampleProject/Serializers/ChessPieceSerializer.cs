using System;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers;

namespace PandoExampleProject.Serializers;

internal class ChessPieceSerializer : INodeSerializer<ChessPiece>
{
	private const int SIZE = 5 * sizeof(byte);
	public int? NodeSize => SIZE;

	public int NodeSizeForObject(ChessPiece obj) => SIZE;

	public void Serialize(ChessPiece obj, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		// We *could* use an EnumSerializer for each of these enums, though I think in this case that would be less readable
		// when writing the serializer manually since the enums can just be directly cast to the underlying type
		writeBuffer[0] = (byte)obj.Owner;
		writeBuffer[1] = (byte)obj.Type;
		writeBuffer[2] = (byte)obj.CurrentRank;
		writeBuffer[3] = (byte)obj.CurrentFile;
		writeBuffer[4] = (byte)obj.State;
	}

	public ChessPiece Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		return new ChessPiece(
			(Player)readBuffer[0],
			(PieceType)readBuffer[1],
			(Rank)readBuffer[2],
			(File)readBuffer[3],
			(ChessPieceState)readBuffer[4]
		);
	}
}
