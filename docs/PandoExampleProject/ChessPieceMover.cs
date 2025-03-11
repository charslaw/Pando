using System;

namespace PandoExampleProject;

internal static class ChessPieceMover
{
	public static ChessGameState MovePiece(
		in ChessGameState startState,
		Player player,
		Index pieceIndex,
		Rank newRank,
		File newFile,
		TimeSpan elapsed = default
	)
	{
		if (player != startState.PlayerState.CurrentTurn)
			throw new ArgumentException($"It is not {player}'s turn!", nameof(player));

		var newPlayerPieces = startState.PlayerPieces.MutateSide(
			player,
			pieces =>
			{
				var movedPiece = pieces[pieceIndex] with { CurrentRank = newRank, CurrentFile = newFile };
				var updatedPieces = (ChessPiece[])pieces.Clone();
				updatedPieces[pieceIndex] = movedPiece;
				return updatedPieces;
			}
		);

		var newRemainingTime = startState.RemainingTime.MutateSide(player, remainingTime => remainingTime - elapsed);

		return new ChessGameState(
			PlayerState: startState.PlayerState with
			{
				CurrentTurn = OtherPlayer(player),
			},
			RemainingTime: newRemainingTime,
			PlayerPieces: newPlayerPieces
		);
	}

	private static Player OtherPlayer(Player player) =>
		player switch
		{
			Player.White => Player.Black,
			Player.Black => Player.White,
			_ => throw new ArgumentOutOfRangeException(nameof(player), player, null),
		};
}
