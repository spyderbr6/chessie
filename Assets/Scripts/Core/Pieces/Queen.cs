using System.Collections.Generic;
using ChessGame.Core.Board;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.Pieces
{
    /// <summary>
    /// Queen piece - combines Rook and Bishop movement (straight + diagonal lines).
    /// </summary>
    public class Queen : ChessPiece
    {
        protected override void Awake()
        {
            base.Awake();
            type = PieceType.Queen;
        }

        public override List<Move> GetPseudoLegalMoves(ChessBoard board)
        {
            // Queen moves in 8 directions: 4 straight + 4 diagonal
            var directions = new[]
            {
                // Straight (Rook-like)
                (0, 1),    // Up
                (0, -1),   // Down
                (-1, 0),   // Left
                (1, 0),    // Right

                // Diagonal (Bishop-like)
                (1, 1),    // Up-right
                (1, -1),   // Down-right
                (-1, 1),   // Up-left
                (-1, -1)   // Down-left
            };

            return GetSlidingMoves(board, directions);
        }
    }
}
