using System;
using System.Buffers.Binary;
using System.Collections.Immutable;
using Pando;
using Pando.DataSources;

namespace PandoExampleProject.Serializers;

/// This is a manually implemented serializer.
/// Generally, this should probably be implemented by a source generator, with manual overrides where necessary.
/// However for this example we are including a manual serializer for illustrative purposes
///
/// Since this is a branch node, its data is composed of the hashes of all of its child nodes
internal class ChessStateTreeSerializer : IPandoNodeSerializerDeserializer<ChessGameState>
{
	private readonly IPandoNodeSerializerDeserializer<ChessPlayerState> _playerStateSerializer;
	private readonly IPandoNodeSerializerDeserializer<WhiteBlackPair<TimeSpan>> _remainingTimeSerializer;
	private readonly IPandoNodeSerializerDeserializer<WhiteBlackPair<ImmutableArray<ChessPiece>>> _playerPiecesSerializer;

	public ChessStateTreeSerializer(
		IPandoNodeSerializerDeserializer<ChessPlayerState> playerStateSerializer,
		IPandoNodeSerializerDeserializer<WhiteBlackPair<TimeSpan>> remainingTimeSerializer,
		IPandoNodeSerializerDeserializer<WhiteBlackPair<ImmutableArray<ChessPiece>>> playerPiecesSerializer
	)
	{
		_playerStateSerializer = playerStateSerializer;
		_remainingTimeSerializer = remainingTimeSerializer;
		_playerPiecesSerializer = playerPiecesSerializer;
	}

	/// <param name="obj">ChessGameState that we want to serialize to the data sink</param>
	/// <param name="dataSink">Data sink we want to serialize to</param>
	public ulong Serialize(ChessGameState obj, INodeDataSink dataSink)
	{
		// Get the hash of each child node by serializing them to the data sink
		ulong playerStateHash = _playerStateSerializer.Serialize(obj.PlayerState, dataSink);
		ulong remainingTimeHash = _remainingTimeSerializer.Serialize(obj.RemainingTime, dataSink);
		ulong piecesHash = _playerPiecesSerializer.Serialize(obj.PlayerPieces, dataSink);

		// Write each hash to a buffer
		// *Pando always assumes little endian*
		Span<byte> buffer = stackalloc byte[sizeof(ulong) * 3];
		BinaryPrimitives.WriteUInt64LittleEndian(buffer.Slice(0, sizeof(ulong)), playerStateHash);
		BinaryPrimitives.WriteUInt64LittleEndian(buffer.Slice(sizeof(ulong), sizeof(ulong)), remainingTimeHash);
		BinaryPrimitives.WriteUInt64LittleEndian(buffer.Slice(sizeof(ulong) * 2, sizeof(ulong)), piecesHash);

		// Write the branch data to the data sink, returning the resulting hash of this node
		return dataSink.AddNode(buffer);
	}

	/// <param name="bytes">The raw byte data of this branch node</param>
	/// <param name="dataSource">The data source to pull child nodes from</param>
	public ChessGameState Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource)
	{
		// Get child hashes from raw data
		// *Pando always assumes little endian*
		ulong playerStateHash = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(0, sizeof(ulong)));
		ulong remainingTimeHash = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(sizeof(ulong), sizeof(ulong)));
		ulong playerPiecesHash = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(sizeof(ulong) * 2, sizeof(ulong)));

		// Get child nodes from data source
		var playerState = dataSource.GetNode(playerStateHash, _playerStateSerializer);
		var remainingTime = dataSource.GetNode(remainingTimeHash, _remainingTimeSerializer);
		var playerPieces = dataSource.GetNode(playerPiecesHash, _playerPiecesSerializer);

		// Create the final state object
		return new ChessGameState(playerState, remainingTime, playerPieces);
	}
}
