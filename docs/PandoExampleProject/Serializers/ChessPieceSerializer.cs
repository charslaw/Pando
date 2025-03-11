using System;
using Pando.Serialization;
using Pando.Vaults;

namespace PandoExampleProject.Serializers;

/// ChessPiece only contains primitive data in the form of enums, so this is a pretty simple serializer.
internal class ChessPieceSerializer : IPandoSerializer<ChessPiece>
{
	public int SerializedSize => 5;

	/// Sequentially writes each enum member of the chess piece into a buffer and submits the node to the data sink.
	public void Serialize(ChessPiece obj, Span<byte> buffer, INodeVault nodeVault)
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
	public ChessPiece Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault)
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

/// An example chess piece serializer with better storage size characteristics than the basic one provided above
internal class OptimizedChessPieceSerializer : IPandoSerializer<ChessPiece>
{
	public int SerializedSize => 2;

	public void Serialize(ChessPiece value, Span<byte> buffer, INodeVault nodeVault)
	{
		// pack owner (1 bit), state (1 bit), and piece type (3 bits) into 1 byte
		buffer[0] = (byte)value.Owner;
		buffer[0] |= (byte)((int)value.State << 1);
		buffer[0] |= (byte)((int)value.Type << 4);

		// pack rank (3 bits) and file (3 bits) into the top and bottom halves of one byte
		buffer[1] = (byte)((int)value.CurrentRank << 4);
		buffer[1] |= value.CurrentFile - File.A;
	}

	public ChessPiece Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault)
	{
		var owner = (Player)(buffer[0] & 0b0000_0001);
		var state = (ChessPieceState)((buffer[0] & 0b0000_0010) >> 1);
		var type = (PieceType)((buffer[0] & 0b1111_0000) >> 4);

		var rank = (Rank)((buffer[1] & 0b1111_0000) >> 4);
		var file = (File)((buffer[1] & 0b0000_1111) + (byte)File.A);

		return new ChessPiece(owner, type, rank, file, state);
	}
}
