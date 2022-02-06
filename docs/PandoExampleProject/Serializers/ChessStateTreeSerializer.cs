using System;
using System.Buffers.Binary;
using System.Collections.Immutable;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers;

namespace PandoExampleProject.Serializers;

/// This is a manually implemented serializer.
/// Generally, this should probably be implemented by a source generator, with manual overrides where necessary.
/// However for this example we are including a manual serializer for illustrative purposes
/// 
/// Since this is a branch node, its data is composed of the hashes of all of its child nodes
internal class ChessStateTreeSerializer : INodeSerializer<ChessGameState>
{
	private readonly INodeSerializer<ChessPlayerState> _playerStateSerializer;
	private readonly INodeSerializer<WhiteBlackPair<TimeSpan>> _remainingTimeSerializer;
	private readonly INodeSerializer<WhiteBlackPair<ImmutableArray<ChessPiece>>> _playerPiecesSerializer;


	public ChessStateTreeSerializer(
		INodeSerializer<ChessPlayerState> playerStateSerializer,
		INodeSerializer<WhiteBlackPair<TimeSpan>> remainingTimeSerializer,
		INodeSerializer<WhiteBlackPair<ImmutableArray<ChessPiece>>> playerPiecesSerializer
	)
	{
		_playerStateSerializer = playerStateSerializer;
		_remainingTimeSerializer = remainingTimeSerializer;
		_playerPiecesSerializer = playerPiecesSerializer;
	}

	private const int SIZE = sizeof(ulong) * 3;
	public int? NodeSize => SIZE;
	public int NodeSizeForObject(ChessGameState obj) => SIZE;

	/// <param name="obj">ChessGameState that we want to serialize to the data sink</param>
	/// <param name="writeBuffer">
	///     The buffer into which to write the binary representation of the given <paramref name="obj" />
	/// </param>
	/// <param name="dataSink">Data sink we want to serialize to</param>
	public void Serialize(ChessGameState obj, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		var (chessPlayerState, whiteBlackPair, playerPieces) = obj;

		// Get the hash of each child node by serializing them to the data sink
		ulong playerStateHash = _playerStateSerializer.SerializeToHash(chessPlayerState, dataSink);
		ulong remainingTimeHash = _remainingTimeSerializer.SerializeToHash(whiteBlackPair, dataSink);
		ulong piecesHash = _playerPiecesSerializer.SerializeToHash(playerPieces, dataSink);

		// Write each hash to the given write buffer
		// *Pando always assumes little endian*
		BinaryPrimitives.WriteUInt64LittleEndian(writeBuffer.Slice(0, sizeof(ulong)), playerStateHash);
		BinaryPrimitives.WriteUInt64LittleEndian(writeBuffer.Slice(sizeof(ulong), sizeof(ulong)), remainingTimeHash);
		BinaryPrimitives.WriteUInt64LittleEndian(writeBuffer.Slice(sizeof(ulong) * 2, sizeof(ulong)), piecesHash);
	}

	/// <param name="readBuffer">The raw byte data of this branch node</param>
	/// <param name="dataSource">The data source to pull child nodes from</param>
	public ChessGameState Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		// Get child hashes from raw data
		// *Pando always assumes little endian*
		ulong playerStateHash = BinaryPrimitives.ReadUInt64LittleEndian(readBuffer.Slice(0, sizeof(ulong)));
		ulong remainingTimeHash = BinaryPrimitives.ReadUInt64LittleEndian(readBuffer.Slice(sizeof(ulong), sizeof(ulong)));
		ulong playerPiecesHash = BinaryPrimitives.ReadUInt64LittleEndian(readBuffer.Slice(sizeof(ulong) * 2, sizeof(ulong)));

		// Get child nodes from data source
		var playerState = _playerStateSerializer.DeserializeFromHash(playerStateHash, dataSource);
		var remainingTime = _remainingTimeSerializer.DeserializeFromHash(remainingTimeHash, dataSource);
		var playerPieces = _playerPiecesSerializer.DeserializeFromHash(playerPiecesHash, dataSource);

		// Create the final state object
		return new ChessGameState(playerState, remainingTime, playerPieces);
	}
}
