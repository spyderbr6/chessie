using System.Collections.Generic;
using ChessGame.Core.Board;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.Pieces
{
    /// <summary>
    /// Chancellor piece - hybrid of Rook and Knight.
    /// Moves in straight lines like a Rook OR jumps in L-shapes like a Knight.
    /// </summary>
    public class Chancellor : ChessPiece
    {
        protected override void Awake()
        {
            base.Awake();
            type = PieceType.Chancellor;
        }

        public override List<Move> GetPseudoLegalMoves(ChessBoard board)
        {
            List<Move> moves = new List<Move>();

            // Rook-like straight moves
            var straightDirections = new[]
            {
                (0, 1),    // Up
                (0, -1),   // Down
                (-1, 0),   // Left
                (1, 0)     // Right
            };
            moves.AddRange(GetSlidingMoves(board, straightDirections));

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
