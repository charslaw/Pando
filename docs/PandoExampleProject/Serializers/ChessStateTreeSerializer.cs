using System;
using Pando.DataSources;
using Pando.Serialization;

namespace PandoExampleProject.Serializers;

/// This is a manually implemented serializer.
/// Generally, this should probably be implemented by a source generator, with manual overrides where necessary.
/// However for this example we are including a manual serializer for illustrative purposes
///
/// Since this is a branch node, its data is composed of the hashes of all of its child nodes
internal class ChessStateTreeSerializer(
	IPandoSerializer<ChessPlayerState> playerStateSerializer,
	IPandoSerializer<WhiteBlackPair<TimeSpan>> remainingTimeSerializer,
	IPandoSerializer<WhiteBlackPair<ChessPiece[]>> playerPiecesSerializer
) : IPandoSerializer<ChessGameState>
{
	public int SerializedSize => sizeof(ulong);

	/// <param name="state">ChessGameState that we want to serialize to the data sink</param>
	/// <param name="buffer">
	///     The buffer into which to write the binary representation of the given <paramref name="state" />
	/// </param>
	/// <param name="dataStore"></param>
	public void Serialize(ChessGameState state, Span<byte> buffer, INodeDataStore dataStore)
	{
		var childrenSize =
			playerStateSerializer.SerializedSize
			+ remainingTimeSerializer.SerializedSize
			+ playerPiecesSerializer.SerializedSize;
		Span<byte> childrenBuffer = stackalloc byte[childrenSize];

		var (chessPlayerState, whiteBlackPair, playerPieces) = state;

		var remainingTimeStart = playerStateSerializer.SerializedSize;
		var playerPiecesStart = remainingTimeStart + remainingTimeSerializer.SerializedSize;

		playerStateSerializer.Serialize(chessPlayerState, childrenBuffer[..remainingTimeStart], dataStore);
		remainingTimeSerializer.Serialize(
			whiteBlackPair,
			childrenBuffer[remainingTimeStart..playerPiecesStart],
			dataStore
		);
		playerPiecesSerializer.Serialize(playerPieces, childrenBuffer[playerPiecesStart..childrenSize], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	/// <param name="buffer">The raw byte data of this branch node</param>
	/// <param name="dataStore"></param>
	public ChessGameState Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		// load node data into buffer
		var nodeDataSize = dataStore.GetSizeOfNode(buffer);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var remainingTimeStart = playerStateSerializer.SerializedSize;
		var playerPiecesStart = remainingTimeStart + remainingTimeSerializer.SerializedSize;
		var bufferEnd = playerPiecesStart + playerPiecesSerializer.SerializedSize;

		// Deserialize children from buffer
		var playerState = playerStateSerializer.Deserialize(childrenBuffer[..remainingTimeStart], dataStore);
		var remainingTime = remainingTimeSerializer.Deserialize(
			childrenBuffer[remainingTimeStart..playerPiecesStart],
			dataStore
		);
		var playerPieces = playerPiecesSerializer.Deserialize(childrenBuffer[playerPiecesStart..bufferEnd], dataStore);

		return new ChessGameState(playerState, remainingTime, playerPieces);
	}
}
