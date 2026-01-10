using System.Collections.Generic;
using ChessGame.Core.Board;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.Pieces
{
    /// <summary>
    /// Archbishop piece - hybrid of Bishop and Knight.
    /// Moves diagonally like a Bishop OR jumps in L-shapes like a Knight.
    /// </summary>
    public class Archbishop : ChessPiece
    {
        protected override void Awake()
        {
            base.Awake();
            type = PieceType.Archbishop;
        }

        public override List<Move> GetPseudoLegalMoves(ChessBoard board)
        {
            List<Move> moves = new List<Move>();

            // Bishop-like diagonal moves
            var diagonalDirections = new[]
            {
                (1, 1),    // Up-right
                (1, -1),   // Down-right
                (-1, 1),   // Up-left
                (-1, -1)   // Down-left
            };
            moves.AddRange(GetSlidingMoves(board, diagonalDirections));

            // Knight-like L-shaped jumps
            var knightOffsets = new[]
            {
                (2, 1),    // 2 right, 1 up
                (2, -1),   // 2 right, 1 down
                (-2, 1),   // 2 left, 1 up
                (-2, -1),  // 2 left, 1 down
                (1, 2),    // 1 right, 2 up
                (1, -2),   // 1 right, 2 down
                (-1, 2),   // 1 left, 2 up
                (-1, -2)   // 1 left, 2 down
            };
            moves.AddRange(GetJumpingMoves(board, knightOffsets));

            return moves;
        }
    }
}
