using System;
using Pando;
using Pando.DataSources;

namespace PandoExampleProject.Serializers;

internal class ChessPieceSerializer : IPandoNodeSerializerDeserializer<ChessPiece>
{
	public ulong Serialize(ChessPiece obj, INodeDataSink dataSink)
	{
		Span<byte> buffer = stackalloc byte[5];

		buffer[0] = (byte)obj.Owner;
		buffer[1] = (byte)obj.Type;
		buffer[2] = (byte)obj.CurrentRank;
		buffer[3] = (byte)obj.CurrentFile;
		buffer[4] = (byte)obj.State;

		return dataSink.AddNode(buffer);
	}

	public ChessPiece Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource)
	{
		return new ChessPiece(
			(Player)bytes[0],
			(PieceType)bytes[1],
			(Rank)bytes[2],
			(File)bytes[3],
			(ChessPieceState)bytes[4]
		);
	}
}
