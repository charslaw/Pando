using System;
using System.Collections.Immutable;

namespace PandoExampleProject;

internal record ChessGameState(                             // Branch node
	ChessPlayerState PlayerState,                           // -> Blob node
	WhiteBlackPair<TimeSpan> RemainingTime,                 // -> Blob node (in this case, the BlackWhitePair contains raw data elements (TimeSpan))
	WhiteBlackPair<ImmutableArray<ChessPiece>> PlayerPieces // -> Branch node (in this case, the BlackWhitePair contains branch nodes)
);

internal record struct ChessPlayerState( // Blob node
	Player CurrentTurn = Player.White,   // -> Raw data
	Winner Winner = Winner.None          // -> Raw data
);

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
	// This will require a separate Serializer/Deserializer for each usage of BlackWhitePair with a different T.
	T WhiteValue, // -> Could be a branch node, blob node, or raw data
	T BlackValue  // -> Could be a branch node, blob node, or raw data
);

internal enum Player : byte { Black, White }

internal enum Winner : byte { None, Black, White }

internal enum PieceType : byte { King, Queen, Rook, Bishop, Knight, Pawn }

internal enum ChessPieceState : byte { Alive, Captured }

internal enum Rank : byte { One = 1, Two, Three, Four, Five, Six, Seven, Eight }

internal enum File : byte { A = (byte)'A', B, C, D, E, F, G, H }
