using System;
using System.Collections.Immutable;

namespace PandoExampleProject;

internal record ChessGameState(                             // Branch node
	ChessPlayerState PlayerState,                           // -> Blob node
	WhiteBlackPair<TimeSpan> RemainingTime,                 // -> Blob node (in this case, the BlackWhitePair contains raw data elements (TimeSpan))
	WhiteBlackPair<ImmutableArray<ChessPiece>> PlayerPieces // -> Branch node (in this case, the BlackWhitePair contains branch nodes)
);

internal record struct ChessPlayerState( // Blob node
	Player CurrentTurn,                  // -> Raw data
	Winner Winner                        // -> Raw data
)
{
	public static ChessPlayerState Default => new(Player.White, Winner.None);
}

internal record struct ChessPiece(                // Blob node
	Player Owner,                                 // -> Raw data
	PieceType Type,                               // -> Raw data
	Rank CurrentRank,                             // -> Raw data
	File CurrentFile,                             // -> Raw data
	ChessPieceState State = ChessPieceState.Alive // -> Raw data
);

/// BlackWhitePair is a container for a type that should exist for both players
internal record WhiteBlackPair<T>(
	// Could be either a branch node or a blob node depending on the type of T.
	// If this is a branch node (i.e. T represents another node type), it can be covered by a generic serializer.
	// However for blobs (i.e. T is a primitive type), a unique serializer must be constructed for each T.
	T WhiteValue, // -> Could be a branch node, blob node, or raw data
	T BlackValue  // -> Could be a branch node, blob node, or raw data
);

internal enum Player : byte { White, Black }

internal enum Winner : byte { None, Black, White }

internal enum PieceType : byte { King, Queen, Rook, Bishop, Knight, Pawn }

internal enum ChessPieceState : byte { Alive, Captured }

internal enum Rank : byte { One = 1, Two, Three, Four, Five, Six, Seven, Eight }

internal enum File : byte { A = (byte)'A', B, C, D, E, F, G, H }
