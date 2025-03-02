using System;

namespace PandoExampleProject;

internal record ChessGameState( // Branch Node
	ChessPlayerState PlayerState, // > Primitive Node
	WhiteBlackPair<TimeSpan> RemainingTime, // Branch Node (containing Primitive Nodes)
	WhiteBlackPair<ChessPiece[]> PlayerPieces // Branch Node (containing Branch Nodes)
);

internal record struct ChessPlayerState( // Primitive node
	Player CurrentTurn, // > Raw data
	Winner Winner // > Raw data
)
{
	public static ChessPlayerState Default => new(Player.White, Winner.None);
}

internal record struct ChessPiece( // Primitive node
	Player Owner, // > Raw data
	PieceType Type, // > Raw data
	Rank CurrentRank, // > Raw data
	File CurrentFile, // > Raw data
	ChessPieceState State = ChessPieceState.Alive // > Raw data
);

/// BlackWhitePair is a container for a value that exists for both players
internal record WhiteBlackPair<T>(T WhiteValue, T BlackValue)
{
	public WhiteBlackPair<T> MutateSide(Player player, Func<T, T> func) =>
		player switch
		{
			Player.White => this with { WhiteValue = func(WhiteValue) },
			Player.Black => this with { BlackValue = func(BlackValue) },
			_ => throw new ArgumentOutOfRangeException(nameof(player), player, null),
		};
}

internal enum Player : byte
{
	White,
	Black,
}

internal enum Winner : byte
{
	None,
	Black,
	White,
}

internal enum PieceType : byte
{
	King,
	Queen,
	Rook,
	Bishop,
	Knight,
	Pawn,
}

internal enum ChessPieceState : byte
{
	Alive,
	Captured,
}

internal enum Rank : byte
{
	One = 1,
	Two,
	Three,
	Four,
	Five,
	Six,
	Seven,
	Eight,
}

internal enum File : byte
{
	A = (byte)'A',
	B,
	C,
	D,
	E,
	F,
	G,
	H,
}
