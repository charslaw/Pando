using System;

namespace PandoExampleProject;

internal record ChessGameState(             // Branch node
	ChessPlayerState PlayerState,           // -> Blob node
	BlackWhitePair<TimeSpan> RemainingTime, // -> Blob node (in this case, the BlackWhitePair contains raw data elements (TimeSpan)
	ChessPiece[] Pieces                     // -> Branch node (array of blobs)
);

internal record ChessPlayerState( // Blob node
	Player CurrentTurn,           // -> Raw data
	Winner Winner                 // -> Raw data
);

internal record ChessPiece( // Blob node
	Player Owner,           // -> Raw data
	PieceType Type,         // -> Raw data
	Rank CurrentRank,       // -> Raw data
	File CurrentFile        // -> Raw data
);

/// BlackWhitePair is a container for a type that should exist for both players
internal record BlackWhitePair<T>(
	// Could be either a branch node or a blob node depending on the type of T.
	// This will require a separate Serializer/Deserializer for each usage of BlackWhitePair with a different T.
	T BlackValue, // -> Could be a branch node, blob node, or raw data
	T WhiteValue  // -> Could be a branch node, blob node, or raw data
);

internal enum Player { Black, White }

internal enum Winner { None, Black, White }

internal enum PieceType { King, Queen, Rook, Bishop, Knight, Pawn }

internal enum Rank { One = 1, Two, Three, Four, Five, Six, Seven, Eight }

internal enum File { A, B, C, D, E, F, G, H }
