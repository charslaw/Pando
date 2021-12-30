using System;
using Pando;
using Pando.DataSources;

namespace PandoExampleProject.Serializers;

internal class ChessPlayerStateSerializer : IPandoNodeSerializerDeserializer<ChessPlayerState>
{
	public ulong Serialize(ChessPlayerState obj, IWritablePandoNodeRepository repository)
	{
		Span<byte> buffer = stackalloc byte[2];

		buffer[0] = (byte)obj.CurrentTurn;
		buffer[1] = (byte)obj.Winner;

		return repository.AddNode(buffer);
	}

	public ChessPlayerState Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository repository)
	{
		return new ChessPlayerState((Player)bytes[0], (Winner)bytes[1]);
	}
}
