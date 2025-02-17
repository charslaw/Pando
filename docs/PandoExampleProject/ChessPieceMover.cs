using System;

namespace PandoExampleProject;

internal static class ChessPieceMover
{
	public static ChessGameState MovePiece(in ChessGameState startState, Player player, Index pieceIndex, Rank newRank, File newFile)
	{
		if (player != startState.PlayerState.CurrentTurn) throw new ArgumentException($"It is not {player}'s turn!", nameof(player));

		var newPlayerPieces = startState.PlayerPieces.MutateSide(player, pieces =>
			{
				var movedPiece = pieces[pieceIndex] with { CurrentRank = newRank, CurrentFile = newFile };
				var updatedPieces = (ChessPiece[])pieces.Clone();
				updatedPieces[pieceIndex] = movedPiece;
				return updatedPieces;
			}
		);

		return startState with
		{
			PlayerState = startState.PlayerState with { CurrentTurn = OtherPlayer(player) },
			PlayerPieces = newPlayerPieces
		};
	}

	private static Player OtherPlayer(Player player) => player switch
	{
		Player.White => Player.Black,
		Player.Black => Player.White,
		_            => throw new ArgumentOutOfRangeException(nameof(player), player, null)
	};
}
