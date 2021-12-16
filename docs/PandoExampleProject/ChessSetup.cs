using System;
using System.Collections.Immutable;

namespace PandoExampleProject;

internal static class ChessSetup
{
	public static ChessGameState InitialGameState() => new(
		ChessPlayerState.Default,
		new WhiteBlackPair<TimeSpan>(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5)),
		InitialBoardState()
	);

	public static WhiteBlackPair<ImmutableArray<ChessPiece>> InitialBoardState() => new(
		ImmutableArray.Create(new ChessPiece[]
			{
				new(Player.White, PieceType.Pawn, Rank.Two, File.A),
				new(Player.White, PieceType.Pawn, Rank.Two, File.B),
				new(Player.White, PieceType.Pawn, Rank.Two, File.C),
				new(Player.White, PieceType.Pawn, Rank.Two, File.D),
				new(Player.White, PieceType.Pawn, Rank.Two, File.E),
				new(Player.White, PieceType.Pawn, Rank.Two, File.F),
				new(Player.White, PieceType.Pawn, Rank.Two, File.G),
				new(Player.White, PieceType.Pawn, Rank.Two, File.H),
				new(Player.White, PieceType.Rook, Rank.One, File.A),
				new(Player.White, PieceType.Knight, Rank.One, File.B),
				new(Player.White, PieceType.Bishop, Rank.One, File.C),
				new(Player.White, PieceType.Queen, Rank.One, File.D),
				new(Player.White, PieceType.King, Rank.One, File.E),
				new(Player.White, PieceType.Bishop, Rank.One, File.F),
				new(Player.White, PieceType.Knight, Rank.One, File.G),
				new(Player.White, PieceType.Rook, Rank.One, File.H),
			}
		),
		ImmutableArray.Create(new ChessPiece[]
			{
				new(Player.Black, PieceType.Pawn, Rank.Seven, File.A),
				new(Player.Black, PieceType.Pawn, Rank.Seven, File.B),
				new(Player.Black, PieceType.Pawn, Rank.Seven, File.C),
				new(Player.Black, PieceType.Pawn, Rank.Seven, File.D),
				new(Player.Black, PieceType.Pawn, Rank.Seven, File.E),
				new(Player.Black, PieceType.Pawn, Rank.Seven, File.F),
				new(Player.Black, PieceType.Pawn, Rank.Seven, File.G),
				new(Player.Black, PieceType.Pawn, Rank.Seven, File.H),
				new(Player.Black, PieceType.Rook, Rank.Eight, File.A),
				new(Player.Black, PieceType.Knight, Rank.Eight, File.B),
				new(Player.Black, PieceType.Bishop, Rank.Eight, File.C),
				new(Player.Black, PieceType.Queen, Rank.Eight, File.D),
				new(Player.Black, PieceType.King, Rank.Eight, File.E),
				new(Player.Black, PieceType.Bishop, Rank.Eight, File.F),
				new(Player.Black, PieceType.Knight, Rank.Eight, File.G),
				new(Player.Black, PieceType.Rook, Rank.Eight, File.H),
			}
		)
	);
}
