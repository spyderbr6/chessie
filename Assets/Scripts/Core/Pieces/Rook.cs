using System.Collections.Generic;
using ChessGame.Core.Board;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.Pieces
{
    /// <summary>
    /// Rook piece - moves in straight lines (horizontal/vertical) until blocked.
    /// </summary>
    public class Rook : ChessPiece
    {
        protected override void Awake()
        {
            base.Awake();
            type = PieceType.Rook;
        }

        public override List<Move> GetPseudoLegalMoves(ChessBoard board)
        {
            // Rook moves in 4 straight directions: up, down, left, right
            var directions = new[]
            {
                (0, 1),   // Up
                (0, -1),  // Down
                (-1, 0),  // Left
                (1, 0)    // Right
            };

            return GetSlidingMoves(board, directions);
        }
    }
}
