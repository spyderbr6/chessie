using System.Collections.Generic;
using ChessGame.Core.Board;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.Pieces
{
    /// <summary>
    /// Bishop piece - moves in diagonal lines until blocked.
    /// </summary>
    public class Bishop : ChessPiece
    {
        protected override void Awake()
        {
            base.Awake();
            type = PieceType.Bishop;
        }

        public override List<Move> GetPseudoLegalMoves(ChessBoard board)
        {
            // Bishop moves in 4 diagonal directions
            var directions = new[]
            {
                (1, 1),    // Up-right
                (1, -1),   // Down-right
                (-1, 1),   // Up-left
                (-1, -1)   // Down-left
            };

            return GetSlidingMoves(board, directions);
        }
    }
}
