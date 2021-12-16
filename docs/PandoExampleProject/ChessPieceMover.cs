using System;

namespace PandoExampleProject;

internal static class ChessPieceMover
{
	public static ChessGameState MovePiece(in ChessGameState startState, Player player, Index pieceIndex, Rank newRank, File newFile)
	{
		if (player != startState.PlayerState.CurrentTurn) throw new ArgumentException($"It is not {player}'s turn!", nameof(player));

		var pieceArray = player switch
		{
			Player.White => startState.PlayerPieces.WhiteValue,
			Player.Black => startState.PlayerPieces.BlackValue,
			_            => throw new ArgumentOutOfRangeException(nameof(player), player, null)
		};
		var newPiece = pieceArray[pieceIndex] with { CurrentRank = newRank, CurrentFile = newFile };
		var newPlayerPieces = player switch
		{
			Player.White => startState.PlayerPieces with { WhiteValue = pieceArray.SetItem(pieceIndex.Value, newPiece) },
			Player.Black => startState.PlayerPieces with { BlackValue = pieceArray.SetItem(pieceIndex.Value, newPiece) },
			_            => throw new ArgumentOutOfRangeException(nameof(player), player, null)
		};

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
