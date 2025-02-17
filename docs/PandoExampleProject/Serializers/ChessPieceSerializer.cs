using System;
using Pando.DataSources;
using Pando.Serialization;

namespace PandoExampleProject.Serializers;

/// ChessPiece only contains primitive data in the form of enums, so this is a pretty simple serializer.
internal class ChessPieceSerializer : IPandoSerializer<ChessPiece>
{
	public int SerializedSize => 5;

	/// Sequentially writes each enum member of the chess piece into a buffer and submits the node to the data sink.
	public void Serialize(ChessPiece obj, Span<byte> buffer, INodeDataSink dataSink)
	{
		// We *could* use an EnumSerializer for each of these enums, though I think in this case that would be less readable
		// when writing the serializer manually since the enums can just be directly cast to the underlying type
		buffer[0] = (byte)obj.Owner;
		buffer[1] = (byte)obj.Type;
		buffer[2] = (byte)obj.CurrentRank;
		buffer[3] = (byte)obj.CurrentFile;
		buffer[4] = (byte)obj.State;
	}

	/// Gets the sequential byte values and converts them to their enum values, then creates a chess piece.
	public ChessPiece Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		return new ChessPiece(
			(Player)buffer[0],
			(PieceType)buffer[1],
			(Rank)buffer[2],
			(File)buffer[3],
			(ChessPieceState)buffer[4]
		);
	}
}
