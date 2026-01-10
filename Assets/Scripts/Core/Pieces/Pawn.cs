using System.Collections.Generic;
using ChessGame.Core.Board;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.Pieces
{
    /// <summary>
    /// Pawn piece - moves forward, captures diagonally.
    /// Special rules: double move on first move, en passant, promotion.
    /// </summary>
    public class Pawn : ChessPiece
    {
        protected override void Awake()
        {
            base.Awake();
            type = PieceType.Pawn;
        }

        public override List<Move> GetPseudoLegalMoves(ChessBoard board)
        {
            List<Move> moves = new List<Move>();

            // Direction depends on color: White moves up (+1), Black moves down (-1)
            int direction = Color == PieceColor.White ? 1 : -1;

            // Forward move (1 square)
            BoardPosition oneForward = Position + (0, direction);
            if (oneForward.IsValid() && board.IsEmpty(oneForward))
            {
                // Check if this move would result in promotion
                int promotionRank = Color == PieceColor.White ? 7 : 0;
                if (oneForward.Rank == promotionRank)
                {
                    // Add promotion moves (handled by SpecialMoveHandler, but mark as promotion)
                    moves.Add(Move.CreatePromotion(Position, oneForward, this, PieceType.Queen));
                }
                else
                {
                    moves.Add(new Move(Position, oneForward, this));
                }

                // Double move on first move (only if single move is clear)
                if (!HasMoved)
                {
                    BoardPosition twoForward = Position + (0, direction * 2);
                    if (twoForward.IsValid() && board.IsEmpty(twoForward))
                    {
                        moves.Add(new Move(Position, twoForward, this));
                    }
                }
            }

            // Diagonal captures (left and right)
            var captureOffsets = new[] { (-1, direction), (1, direction) };

            foreach (var (dFile, dRank) in captureOffsets)
            {
                BoardPosition capturePos = Position + (dFile, dRank);

                if (!capturePos.IsValid())
                    continue;

                ChessPiece targetPiece = board.GetPiece(capturePos);

                // Can capture enemy piece
                if (targetPiece != null && targetPiece.Color != this.Color)
                {
                    // Check if capture would result in promotion
                    int promotionRank = Color == PieceColor.White ? 7 : 0;
                    if (capturePos.Rank == promotionRank)
                    {
                        moves.Add(Move.CreatePromotion(Position, capturePos, this, PieceType.Queen, targetPiece));
                    }
                    else
                    {
                        moves.Add(new Move(Position, capturePos, this, targetPiece, MoveType.Capture));
                    }
                }

                // En passant is handled by SpecialMoveHandler (checks en passant target square)
            }

            return moves;
        }

        /// <summary>
        /// Checks if this pawn is on its starting rank.
        /// </summary>
        public bool IsOnStartingRank()
        {
            int startingRank = Color == PieceColor.White ? 1 : 6;
            return Position.Rank == startingRank;
        }

        /// <summary>
        /// Checks if this pawn is on the promotion rank.
        /// </summary>
        public bool IsOnPromotionRank()
        {
            int promotionRank = Color == PieceColor.White ? 7 : 0;
            return Position.Rank == promotionRank;
        }
    }
}
