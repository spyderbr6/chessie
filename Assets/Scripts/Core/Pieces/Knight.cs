using System.Collections.Generic;
using ChessGame.Core.Board;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.Pieces
{
    /// <summary>
    /// Knight piece - moves in L-shapes (2+1 squares), can jump over other pieces.
    /// </summary>
    public class Knight : ChessPiece
    {
        protected override void Awake()
        {
            base.Awake();
            type = PieceType.Knight;
        }

        public override List<Move> GetPseudoLegalMoves(ChessBoard board)
        {
            // Knight has 8 possible L-shaped moves: 2 squares in one direction, 1 in perpendicular
            var offsets = new[]
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

            return GetJumpingMoves(board, offsets);
        }
    }
}
