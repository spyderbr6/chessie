using System.Collections.Generic;
using ChessGame.Core.Board;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.Pieces
{
    /// <summary>
    /// King piece - moves one square in any direction.
    /// Castling logic is handled separately in SpecialMoveHandler.
    /// </summary>
    public class King : ChessPiece
    {
        protected override void Awake()
        {
            base.Awake();
            type = PieceType.King;
        }

        public override List<Move> GetPseudoLegalMoves(ChessBoard board)
        {
            // King can move one square in any of 8 directions
            var offsets = new[]
            {
                (0, 1),    // Up
                (0, -1),   // Down
                (-1, 0),   // Left
                (1, 0),    // Right
                (1, 1),    // Up-right
                (1, -1),   // Down-right
                (-1, 1),   // Up-left
                (-1, -1)   // Down-left
            };

            List<Move> moves = new List<Move>();

            foreach (var (dFile, dRank) in offsets)
            {
                BoardPosition targetPos = Position + (dFile, dRank);

                if (!targetPos.IsValid())
                    continue;

                ChessPiece pieceAtTarget = board.GetPiece(targetPos);

                if (pieceAtTarget == null)
                {
                    // Empty square
                    moves.Add(new Move(Position, targetPos, this));
                }
                else if (pieceAtTarget.Color != this.Color)
                {
                    // Enemy piece
                    moves.Add(new Move(Position, targetPos, this, pieceAtTarget, MoveType.Capture));
                }
                // Friendly piece - can't move there
            }

            // Note: Castling moves are added by SpecialMoveHandler, not here
            return moves;
        }
    }
}
