using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Pando.Persistors;
using Pando.Repositories;
using Pando.Serializers;
using Pando.Serializers.Collections;
using Pando.Serializers.Primitives;
using Pando.Vaults;
using PandoExampleProject.Serializers;

namespace PandoExampleProject;

/// These tests are more to illustrate the usage of a repository and an example state tree
/// than to assert functionality about the system.
public class ChessTests
{
	private static IPandoSerializer<ChessGameState> MakeSerializer() =>
		new ChessStateTreeSerializer(
			new ChessPlayerStateSerializer(),
			new WhiteBlackPairSerializer<TimeSpan>(TimeSpanTicksSerializer.Default),
			new WhiteBlackPairSerializer<ChessPiece[]>(new ArraySerializer<ChessPiece>(new ChessPieceSerializer()))
		);

	[Test]
	public async Task MakeSomeMoves()
	{
		// Initialize a pando repository that stores data in memory and uses our serializer
		var pandoRepository = new PandoRepository<ChessGameState>(
			new MemoryNodeVault(),
			new MemorySnapshotVault(),
			MakeSerializer()
		);

		// Initial State
		var initialState = ChessSetup.InitialGameState();
		var initialHash = pandoRepository.SaveRootSnapshot(initialState);

		var initialExpected = """
			♜♞♝♛♚♝♞♜ 8
			♟♟♟♟♟♟♟♟ 7
			￣＃￣＃￣＃￣＃ 6
			＃￣＃￣＃￣＃￣ 5
			￣＃￣＃￣＃￣＃ 4
			＃￣＃￣＃￣＃￣ 3
			♙♙♙♙♙♙♙♙ 2
			♖♘♗♕♔♗♘♖ 1
			ＡＢＣＤＥＦＧＨ
			""".ReplaceLineEndings(Environment.NewLine);

		var initialActual = ChessBoardRenderer.RenderBoard(pandoRepository.GetSnapshot(initialHash));
		await Assert.That(initialActual).IsEqualTo(initialExpected);

		// First move: e4
		var firstMove = ChessPieceMover.MovePiece(initialState, Player.White, 4, Rank.Four, File.E);
		var firstMoveHash = pandoRepository.SaveSnapshot(firstMove, initialHash);

		var firstMoveExpected = """
			♜♞♝♛♚♝♞♜ 8
			♟♟♟♟♟♟♟♟ 7
			￣＃￣＃￣＃￣＃ 6
			＃￣＃￣＃￣＃￣ 5
			￣＃￣＃♙＃￣＃ 4
			＃￣＃￣＃￣＃￣ 3
			♙♙♙♙￣♙♙♙ 2
			♖♘♗♕♔♗♘♖ 1
			ＡＢＣＤＥＦＧＨ
			""".ReplaceLineEndings(Environment.NewLine);

		var firstMoveActual = ChessBoardRenderer.RenderBoard(pandoRepository.GetSnapshot(firstMoveHash));

		await Assert.That(firstMoveActual).IsEqualTo(firstMoveExpected);
	}

	[Test]
	public async Task SaveToJson()
	{
		var stream = new MemoryStream();
		var jsonPersistor = JsonNodePersistor.CreateFromStream(stream);

		var repository = new PandoRepository<ChessGameState>(
			new MemoryNodeVault(jsonPersistor),
			new MemorySnapshotVault(),
			MakeSerializer()
		);

		var initialState = ChessSetup.InitialGameState();
		var initialSnapshot = repository.SaveRootSnapshot(initialState);

		var initialJson = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);

		// A single chess game state snapshot consists of 3 nodes:
		// - The first and second node store the pieces for white and black, respectively.
		//   Each player has 16 pieces and each piece is 5 bytes.
		// - The third node is the root node.
		//   The first byte is the current player and the second byte is the winner.
		//   The next 16 bytes store the remaining time for white and black, respectively.
		//   The last 16 bytes store a reference to the nodes for white and black's pieces, respectively.
		var expectedInitialJson = """
			{
			  "685ae5b45d51f705": "0005024100000502420000050243000005024400000502450000050246000005024700000502480000020141000004014200000301430000010144000000014500000301460000040147000002014800",
			  "16a1d0025283dba9": "0105074100010507420001050743000105074400010507450001050746000105074700010507480001020841000104084200010308430001010844000100084500010308460001040847000102084800",
			  "5be63be903c87808": "0000005ED0B200000000005ED0B200000000685AE5B45D51F70516A1D0025283DBA9"
			}
			""".ReplaceLineEndings(Environment.NewLine);

		await Assert.That(initialJson).IsEqualTo(expectedInitialJson);

		var firstMove = ChessPieceMover.MovePiece(
			initialState,
			Player.White,
			4,
			Rank.Four,
			File.E,
			TimeSpan.FromSeconds(2.54)
		);
		_ = repository.SaveSnapshot(firstMove, initialSnapshot);

		var firstMoveJson = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);

		// Here we can see 2 new nodes added:
		// - The fourth node is the new array of white's pieces with the position of the E file pawn moved to the 4th rank
		// - The fifth node is the new root node with the new current player, white's remaining time,
		//   and referencing the updated white pieces array.
		var expectedFirstMoveJson = """
			{
			  "685ae5b45d51f705": "0005024100000502420000050243000005024400000502450000050246000005024700000502480000020141000004014200000301430000010144000000014500000301460000040147000002014800",
			  "16a1d0025283dba9": "0105074100010507420001050743000105074400010507450001050746000105074700010507480001020841000104084200010308430001010844000100084500010308460001040847000102084800",
			  "5be63be903c87808": "0000005ED0B200000000005ED0B200000000685AE5B45D51F70516A1D0025283DBA9",
			  "2965c8539de6f372": "0005024100000502420000050243000005024400000504450000050246000005024700000502480000020141000004014200000301430000010144000000014500000301460000040147000002014800",
			  "9b6024556565dff8": "010040CB4CB100000000005ED0B2000000002965C8539DE6F37216A1D0025283DBA9"
			}
			""".ReplaceLineEndings(Environment.NewLine);

		await Assert.That(firstMoveJson).IsEqualTo(expectedFirstMoveJson);
	}
}
