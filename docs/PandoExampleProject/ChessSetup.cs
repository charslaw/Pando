using System;

namespace PandoExampleProject;

internal static class ChessSetup
{
	public static ChessGameState InitialGameState() =>
		new(
			ChessPlayerState.Default,
			new WhiteBlackPair<TimeSpan>(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5)),
			InitialBoardState()
		);

	private static WhiteBlackPair<ChessPiece[]> InitialBoardState() =>
		new(
			[
				new ChessPiece(Player.White, PieceType.Pawn, Rank.Two, File.A),
				new ChessPiece(Player.White, PieceType.Pawn, Rank.Two, File.B),
				new ChessPiece(Player.White, PieceType.Pawn, Rank.Two, File.C),
				new ChessPiece(Player.White, PieceType.Pawn, Rank.Two, File.D),
				new ChessPiece(Player.White, PieceType.Pawn, Rank.Two, File.E),
				new ChessPiece(Player.White, PieceType.Pawn, Rank.Two, File.F),
				new ChessPiece(Player.White, PieceType.Pawn, Rank.Two, File.G),
				new ChessPiece(Player.White, PieceType.Pawn, Rank.Two, File.H),
				new ChessPiece(Player.White, PieceType.Rook, Rank.One, File.A),
				new ChessPiece(Player.White, PieceType.Knight, Rank.One, File.B),
				new ChessPiece(Player.White, PieceType.Bishop, Rank.One, File.C),
				new ChessPiece(Player.White, PieceType.Queen, Rank.One, File.D),
				new ChessPiece(Player.White, PieceType.King, Rank.One, File.E),
				new ChessPiece(Player.White, PieceType.Bishop, Rank.One, File.F),
				new ChessPiece(Player.White, PieceType.Knight, Rank.One, File.G),
				new ChessPiece(Player.White, PieceType.Rook, Rank.One, File.H),
			],
			[
				new ChessPiece(Player.Black, PieceType.Pawn, Rank.Seven, File.A),
				new ChessPiece(Player.Black, PieceType.Pawn, Rank.Seven, File.B),
				new ChessPiece(Player.Black, PieceType.Pawn, Rank.Seven, File.C),
				new ChessPiece(Player.Black, PieceType.Pawn, Rank.Seven, File.D),
				new ChessPiece(Player.Black, PieceType.Pawn, Rank.Seven, File.E),
				new ChessPiece(Player.Black, PieceType.Pawn, Rank.Seven, File.F),
				new ChessPiece(Player.Black, PieceType.Pawn, Rank.Seven, File.G),
				new ChessPiece(Player.Black, PieceType.Pawn, Rank.Seven, File.H),
				new ChessPiece(Player.Black, PieceType.Rook, Rank.Eight, File.A),
				new ChessPiece(Player.Black, PieceType.Knight, Rank.Eight, File.B),
				new ChessPiece(Player.Black, PieceType.Bishop, Rank.Eight, File.C),
				new ChessPiece(Player.Black, PieceType.Queen, Rank.Eight, File.D),
				new ChessPiece(Player.Black, PieceType.King, Rank.Eight, File.E),
				new ChessPiece(Player.Black, PieceType.Bishop, Rank.Eight, File.F),
				new ChessPiece(Player.Black, PieceType.Knight, Rank.Eight, File.G),
				new ChessPiece(Player.Black, PieceType.Rook, Rank.Eight, File.H),
			]
		);
}
