using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Pando.DataSources;
using Pando.Repositories;
using Pando.Serialization.NodeSerializers;
using Pando.Serialization.NodeSerializers.EnumerableFactory;
using Pando.Serialization.PrimitiveSerializers;
using PandoExampleProject.Serializers;
using Xunit;

namespace PandoExampleProject;

/// These tests are more to illustrate the usage of a repository and an example state tree
/// than to assert functionality about the system.
public class ChessTests
{
	private static INodeSerializer<ChessGameState> MakeSerializer() => new ChessStateTreeSerializer(
		new ChessPlayerStateSerializer(),
		new PrimitiveWhiteBlackPairSerializer<TimeSpan>(new TimeSpanTicksSerializer()),
		new NodeWhiteBlackPairSerializer<ImmutableArray<ChessPiece>>(
			new NodeListSerializer<ImmutableArray<ChessPiece>, ChessPiece>(
				new ChessPieceSerializer(), new ImmutableArrayFactory<ChessPiece>()
			)
		)
	);

	[Fact]
	public void MakeSomeMoves()
	{
		var pandoRepository = new PandoRepository<ChessGameState>(
			new MemoryDataSource(),
			MakeSerializer()
		);

		// Initial State
		var initialState = ChessSetup.InitialGameState();
		var initialHash = pandoRepository.SaveRootSnapshot(initialState);

		var initialExpected = TrimBoardLines(@"
			♜♞♝♛♚♝♞♜ 8
			♟♟♟♟♟♟♟♟ 7
			￣＃￣＃￣＃￣＃ 6
			＃￣＃￣＃￣＃￣ 5
			￣＃￣＃￣＃￣＃ 4
			＃￣＃￣＃￣＃￣ 3
			♙♙♙♙♙♙♙♙ 2
			♖♘♗♕♔♗♘♖ 1
			ＡＢＣＤＥＦＧＨ
		"
		);

		var initialActual = ChessBoardRenderer.RenderBoard(pandoRepository.GetSnapshot(initialHash));
		Assert.Equal(initialExpected, initialActual);

		// First move: e4
		var firstMove = ChessPieceMover.MovePiece(initialState, Player.White, 4, Rank.Four, File.E);
		var firstMoveHash = pandoRepository.SaveSnapshot(firstMove, initialHash);

		var firstMoveExpected = TrimBoardLines(@"
			♜♞♝♛♚♝♞♜ 8
			♟♟♟♟♟♟♟♟ 7
			￣＃￣＃￣＃￣＃ 6
			＃￣＃￣＃￣＃￣ 5
			￣＃￣＃♙＃￣＃ 4
			＃￣＃￣＃￣＃￣ 3
			♙♙♙♙￣♙♙♙ 2
			♖♘♗♕♔♗♘♖ 1
			ＡＢＣＤＥＦＧＨ
		"
		);

		var firstMoveActual = ChessBoardRenderer.RenderBoard(pandoRepository.GetSnapshot(firstMoveHash));
		Assert.Equal(firstMoveExpected, firstMoveActual);
	}

	private static string TrimBoardLines(string board) => Regex.Replace(board.Trim(), @"\s*\n\s*", "\n");
}
