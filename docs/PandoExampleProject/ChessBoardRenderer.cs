using System;
using System.Collections.Generic;

namespace PandoExampleProject;

internal static class ChessBoardRenderer
{
	private const char LIGHT_SQUARE = '￣';
	private const char DARK_SQUARE = '＃';

	private const char WHITE_PAWN = '♙';
	private const char WHITE_KNIGHT = '♘';
	private const char WHITE_BISHOP = '♗';
	private const char WHITE_ROOK = '♖';
	private const char WHITE_QUEEN = '♕';
	private const char WHITE_KING = '♔';

	private const char BLACK_PAWN = '♟';
	private const char BLACK_KNIGHT = '♞';
	private const char BLACK_BISHOP = '♝';
	private const char BLACK_ROOK = '♜';
	private const char BLACK_QUEEN = '♛';
	private const char BLACK_KING = '♚';

	public static string RenderBoard(ChessGameState gameState)
	{
		char[][] boardChars = new char[Rank.Eight - Rank.One + 1][];

		for (Rank rank = Rank.One; rank <= Rank.Eight; rank++)
		{
			var rankIndex = rank - Rank.One;
			boardChars[rankIndex] = new char[File.H - File.A + 1];
			for (File file = File.A; file <= File.H; file++)
			{
				var fileIndex = file - File.A;
				boardChars[rankIndex][fileIndex] = (rankIndex + fileIndex) % 2 == 0 ? DARK_SQUARE : LIGHT_SQUARE;
			}
		}

		List<ChessPiece> allPieces = new List<ChessPiece>(32);
		allPieces.AddRange(gameState.PlayerPieces.WhiteValue);
		allPieces.AddRange(gameState.PlayerPieces.BlackValue);
		foreach (var piece in allPieces)
		{
			if (piece.State == ChessPieceState.Captured)
				continue;

			var rankIndex = piece.CurrentRank - Rank.One;
			var fileIndex = piece.CurrentFile - File.A;
			boardChars[rankIndex][fileIndex] = GetPieceChar(piece);
		}

		List<string> lines = new List<string>(9);

		for (Rank rank = Rank.Eight; rank >= Rank.One; rank--)
		{
			var rankIndex = rank - Rank.One;
			lines.Add($"{string.Join("", boardChars[rankIndex])} {(int)rank}");
		}

		lines.Add("ＡＢＣＤＥＦＧＨ");

		return string.Join(Environment.NewLine, lines);
	}

	private static char GetPieceChar(ChessPiece piece) =>
		piece switch
		{
			(Player.White, PieceType.Pawn, _, _, _) => WHITE_PAWN,
			(Player.White, PieceType.Knight, _, _, _) => WHITE_KNIGHT,
			(Player.White, PieceType.Bishop, _, _, _) => WHITE_BISHOP,
			(Player.White, PieceType.Rook, _, _, _) => WHITE_ROOK,
			(Player.White, PieceType.Queen, _, _, _) => WHITE_QUEEN,
			(Player.White, PieceType.King, _, _, _) => WHITE_KING,
			(Player.Black, PieceType.Pawn, _, _, _) => BLACK_PAWN,
			(Player.Black, PieceType.Knight, _, _, _) => BLACK_KNIGHT,
			(Player.Black, PieceType.Bishop, _, _, _) => BLACK_BISHOP,
			(Player.Black, PieceType.Rook, _, _, _) => BLACK_ROOK,
			(Player.Black, PieceType.Queen, _, _, _) => BLACK_QUEEN,
			(Player.Black, PieceType.King, _, _, _) => BLACK_KING,
			_ => throw new ArgumentException($"Unknown piece: {piece.Owner} {piece.Type}", nameof(piece)),
		};
}
