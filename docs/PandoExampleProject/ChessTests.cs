using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Pando;
using Pando.DataSources;
using PandoExampleProject.Serializers;
using Xunit;

namespace PandoExampleProject;

/// These tests are more to illustrate the usage of a PandoSaver and an example state tree
/// than to assert functionality about the system.
public class ChessTests
{
	private static INodeSerializer<ChessGameState> MakeSerializer() => new ChessStateTreeSerializer(
		new ChessPlayerStateSerializer(),
		new WhiteBlackPairTimespanSerializer(),
		new WhiteBlackPairBranchSerializer<ImmutableArray<ChessPiece>>(
			new ImmutableArraySerializer<ChessPiece>(
				new ChessPieceSerializer()
			)
		)
	);

	[Fact]
	public void MakeSomeMoves()
	{
		var pandoSaver = new PandoSaver<ChessGameState>(
			new MemoryDataSource(),
			MakeSerializer()
		);

		// Initial State
		var initialState = ChessSetup.InitialGameState();
		var initialHash = pandoSaver.SaveRootSnapshot(initialState);

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

		var initialActual = ChessBoardRenderer.RenderBoard(pandoSaver.GetSnapshot(initialHash));
		Assert.Equal(initialExpected, initialActual);

		// First move: e4
		var firstMove = ChessPieceMover.MovePiece(initialState, Player.White, 4, Rank.Four, File.E);
		var firstMoveHash = pandoSaver.SaveSnapshot(firstMove, initialHash);

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

		var firstMoveActual = ChessBoardRenderer.RenderBoard(pandoSaver.GetSnapshot(firstMoveHash));
		Assert.Equal(firstMoveExpected, firstMoveActual);
	}

	private static string TrimBoardLines(string board) => Regex.Replace(board.Trim(), @"\s*\n\s*", "\n");
}
