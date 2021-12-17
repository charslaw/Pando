using System;
using Pando;
using Pando.Repositories;

namespace PandoExampleProject.Serializers;

internal class ChessPieceSerializer : IPandoNodeSerializerDeserializer<ChessPiece>
{
	public ulong Serialize(ChessPiece obj, IWritablePandoNodeRepository repository)
	{
		Span<byte> buffer = stackalloc byte[5];

		buffer[0] = (byte)obj.Owner;
		buffer[1] = (byte)obj.Type;
		buffer[2] = (byte)obj.CurrentRank;
		buffer[3] = (byte)obj.CurrentFile;
		buffer[4] = (byte)obj.State;

		return repository.AddNode(buffer);
	}

	public ChessPiece Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository repository)
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
