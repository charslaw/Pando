using System;
using System.Text.RegularExpressions;
using Pando.DataSources;
using Pando.Repositories;
using Pando.Serialization;
using Pando.Serialization.Collections;
using Pando.Serialization.Primitives;
using PandoExampleProject.Serializers;
using Xunit;

namespace PandoExampleProject;

/// These tests are more to illustrate the usage of a repository and an example state tree
/// than to assert functionality about the system.
public class ChessTests
{
	private static IPandoSerializer<ChessGameState> MakeSerializer() => new ChessStateTreeSerializer(
		new ChessPlayerStateSerializer(),
		new WhiteBlackPairSerializer<TimeSpan>(TimeSpanTicksSerializer.Default),
		new WhiteBlackPairSerializer<ChessPiece[]>(
			new ArraySerializer<ChessPiece>(new ChessPieceSerializer())
		)
	);

	[Fact]
	public void MakeSomeMoves()
	{
		// Initialize a pando repository that stores data in memory and uses our serializer
		var pandoRepository = new PandoRepository<ChessGameState>(
			new MemoryNodeStore(),
			new MemorySnapshotStore(),
			MakeSerializer()
		);

		// Initial State
		var initialState = ChessSetup.InitialGameState();
		var initialHash = pandoRepository.SaveRootSnapshot(initialState);

		var initialExpected =
			"""
			♜♞♝♛♚♝♞♜ 8
			♟♟♟♟♟♟♟♟ 7
			￣＃￣＃￣＃￣＃ 6
			＃￣＃￣＃￣＃￣ 5
			￣＃￣＃￣＃￣＃ 4
			＃￣＃￣＃￣＃￣ 3
			♙♙♙♙♙♙♙♙ 2
			♖♘♗♕♔♗♘♖ 1
			ＡＢＣＤＥＦＧＨ
			""";

		var initialActual = ChessBoardRenderer.RenderBoard(pandoRepository.GetSnapshot(initialHash));
		Assert.Equal(initialExpected, initialActual);

		// First move: e4
		var firstMove = ChessPieceMover.MovePiece(initialState, Player.White, 4, Rank.Four, File.E);
		var firstMoveHash = pandoRepository.SaveSnapshot(firstMove, initialHash);

		var firstMoveExpected =
			"""
			♜♞♝♛♚♝♞♜ 8
			♟♟♟♟♟♟♟♟ 7
			￣＃￣＃￣＃￣＃ 6
			＃￣＃￣＃￣＃￣ 5
			￣＃￣＃♙＃￣＃ 4
			＃￣＃￣＃￣＃￣ 3
			♙♙♙♙￣♙♙♙ 2
			♖♘♗♕♔♗♘♖ 1
			ＡＢＣＤＥＦＧＨ
			""";

		var firstMoveActual = ChessBoardRenderer.RenderBoard(pandoRepository.GetSnapshot(firstMoveHash));
		Assert.Equal(firstMoveExpected, firstMoveActual);
	}
}
