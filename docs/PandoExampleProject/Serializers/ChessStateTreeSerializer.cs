using System;
using System.Buffers.Binary;
using System.Collections.Immutable;
using Pando;
using Pando.Repositories;

namespace PandoExampleProject.Serializers;

/// This is a manually implemented serializer.
/// Generally, this should probably be implemented by a source generator, with manual overrides where necessary.
/// However for this example we are including a manual serializer for illustrative purposes
///
/// Since this is a branch node, its data is composed of the hashes of all of its child nodes
internal record ChessStateTreeSerializer(
	IPandoNodeSerializerDeserializer<ChessPlayerState> PlayerStateSerializer,
	IPandoNodeSerializerDeserializer<WhiteBlackPair<TimeSpan>> RemainingTimeSerializer,
	IPandoNodeSerializerDeserializer<WhiteBlackPair<ImmutableArray<ChessPiece>>> PlayerPiecesSerializer
) : IPandoNodeSerializerDeserializer<ChessGameState>
{
	/// <param name="obj">ChessGameState that we want to serialize to the repository</param>
	/// <param name="repository">Repository we want to serialize to</param>
	public ulong Serialize(ChessGameState obj, IWritablePandoNodeRepository repository)
	{
		// Get the hash of each child node by serializing them to the repository
		ulong playerStateHash = PlayerStateSerializer.Serialize(obj.PlayerState, repository);
		ulong remainingTimeHash = RemainingTimeSerializer.Serialize(obj.RemainingTime, repository);
		ulong piecesHash = PlayerPiecesSerializer.Serialize(obj.PlayerPieces, repository);

		// Write each hash to a buffer
		// *Pando always assumes little endian*
		Span<byte> buffer = stackalloc byte[sizeof(ulong) * 3];
		BinaryPrimitives.WriteUInt64LittleEndian(buffer.Slice(0, sizeof(ulong)), playerStateHash);
		BinaryPrimitives.WriteUInt64LittleEndian(buffer.Slice(sizeof(ulong), sizeof(ulong)), remainingTimeHash);
		BinaryPrimitives.WriteUInt64LittleEndian(buffer.Slice(sizeof(ulong) * 2, sizeof(ulong)), piecesHash);

		// Write the branch data to the repository, returning the resulting hash of this node
		return repository.AddNode(buffer);
	}

	/// <param name="bytes">The raw byte data of this branch node</param>
	/// <param name="repository">The repository to pull child nodes from</param>
	public ChessGameState Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository repository)
	{
		// Get child hashes from raw data
		// *Pando always assumes little endian*
		ulong playerStateHash = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(0, sizeof(ulong)));
		ulong remainingTimeHash = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(sizeof(ulong), sizeof(ulong)));
		ulong playerPiecesHash = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(sizeof(ulong) * 2, sizeof(ulong)));

		// Get child nodes from repository
		var playerState = repository.GetNode(playerStateHash, PlayerStateSerializer);
		var remainingTime = repository.GetNode(remainingTimeHash, RemainingTimeSerializer);
		var playerPieces = repository.GetNode(playerPiecesHash, PlayerPiecesSerializer);

		return new ChessGameState(playerState, remainingTime, playerPieces);
	}
}
