using System.Collections.Generic;
using UnityEngine;
using ChessGame.Core.Board;
using ChessGame.Core.Pieces;

namespace ChessGame.Core.MoveSystem
{
    /// <summary>
    /// Handles generation of special chess moves: castling, en passant, and promotion.
    /// Integrates with MoveGenerator to add these moves to pseudo-legal move lists.
    /// </summary>
    public class SpecialMoveHandler : MonoBehaviour
    {
        [SerializeField] private ChessBoard board;
        [SerializeField] private MoveGenerator moveGenerator;

        /// <summary>
        /// Adds castling moves for a king if legal.
        /// Capablanca chess castling positions:
        /// - King starts at file 5 (f-file)
        /// - Queenside: King f→c (5→2), Rook a→d (0→3)
        /// - Kingside: King f→i (5→8), Rook j→h (9→7)
        /// </summary>
        public void AddCastlingMoves(King king, List<Move> moves)
        {
            if (king.HasMoved)
                return; // King has moved, can't castle

            int rank = king.Color == PieceColor.White ? 0 : 7;
            BoardPosition kingPos = king.Position;

            // Verify king is on starting square (file 5)
            if (kingPos.File != 5 || kingPos.Rank != rank)
                return;

            // Check if king is in check (can't castle out of check)
            if (moveGenerator.IsKingInCheck(king.Color))
                return;

            // Try queenside castling
            TryAddQueensideCastling(king, kingPos, rank, moves);

            // Try kingside castling
            TryAddKingsideCastling(king, kingPos, rank, moves);
        }

        /// <summary>
        /// Attempts to add queenside castling move.
        /// King f→c (5→2), Rook a→d (0→3)
        /// </summary>
        private void TryAddQueensideCastling(King king, BoardPosition kingPos, int rank, List<Move> moves)
        {
            // Check if queenside castling is still allowed
            if (!board.CanCastle(king.Color, false))
                return;

            // Check if rook is present at a-file (file 0)
            BoardPosition rookPos = new BoardPosition(0, rank);
            ChessPiece rook = board.GetPiece(rookPos);
            if (rook == null || rook.Type != PieceType.Rook || rook.HasMoved)
                return;

            // Squares between king and rook must be empty: b, c, d (files 1, 2, 3)
            BoardPosition[] emptySquares = new[]
            {
                new BoardPosition(1, rank), // b
                new BoardPosition(2, rank), // c
                new BoardPosition(3, rank)  // d
            };

            foreach (BoardPosition pos in emptySquares)
            {
                if (!board.IsEmpty(pos))
                    return;
            }

            // King must not pass through check: squares f, e, d, c (files 5, 4, 3, 2)
            // King path: f→e→d→c
            BoardPosition[] kingPath = new[]
            {
                new BoardPosition(5, rank), // f (starting square)
                new BoardPosition(4, rank), // e
                new BoardPosition(3, rank), // d
                new BoardPosition(2, rank)  // c (destination)
            };

            PieceColor enemyColor = king.Color == PieceColor.White ? PieceColor.Black : PieceColor.White;
            foreach (BoardPosition pos in kingPath)
            {
                if (moveGenerator.IsSquareAttackedBy(pos, enemyColor))
                    return; // King passes through or ends in check
            }

            // All conditions met - add castling move
            BoardPosition kingDestination = new BoardPosition(2, rank); // c-file
            BoardPosition rookDestination = new BoardPosition(3, rank); // d-file

            Move castlingMove = Move.CreateCastling(kingPos, kingDestination, king, rookPos, rookDestination);
            moves.Add(castlingMove);
        }

        /// <summary>
        /// Attempts to add kingside castling move.
        /// King f→i (5→8), Rook j→h (9→7)
        /// </summary>
        private void TryAddKingsideCastling(King king, BoardPosition kingPos, int rank, List<Move> moves)
        {
            // Check if kingside castling is still allowed
            if (!board.CanCastle(king.Color, true))
                return;

            // Check if rook is present at j-file (file 9)
            BoardPosition rookPos = new BoardPosition(9, rank);
            ChessPiece rook = board.GetPiece(rookPos);
            if (rook == null || rook.Type != PieceType.Rook || rook.HasMoved)
                return;

            // Squares between king and rook must be empty: g, h, i (files 6, 7, 8)
            BoardPosition[] emptySquares = new[]
            {
                new BoardPosition(6, rank), // g
                new BoardPosition(7, rank), // h
                new BoardPosition(8, rank)  // i
            };

            foreach (BoardPosition pos in emptySquares)
            {
                if (!board.IsEmpty(pos))
                    return;
            }

            // King must not pass through check: squares f, g, h, i (files 5, 6, 7, 8)
            // King path: f→g→h→i
            BoardPosition[] kingPath = new[]
            {
                new BoardPosition(5, rank), // f (starting square)
                new BoardPosition(6, rank), // g
                new BoardPosition(7, rank), // h
                new BoardPosition(8, rank)  // i (destination)
            };

            PieceColor enemyColor = king.Color == PieceColor.White ? PieceColor.Black : PieceColor.White;
            foreach (BoardPosition pos in kingPath)
            {
                if (moveGenerator.IsSquareAttackedBy(pos, enemyColor))
                    return; // King passes through or ends in check
            }

            // All conditions met - add castling move
            BoardPosition kingDestination = new BoardPosition(8, rank); // i-file
            BoardPosition rookDestination = new BoardPosition(7, rank); // h-file

            Move castlingMove = Move.CreateCastling(kingPos, kingDestination, king, rookPos, rookDestination);
            moves.Add(castlingMove);
        }

        /// <summary>
        /// Adds en passant capture moves for a pawn if legal.
        /// </summary>
        public void AddEnPassantMoves(Pawn pawn, List<Move> moves)
        {
            BoardPosition? enPassantTarget = board.GetEnPassantTarget();
            if (!enPassantTarget.HasValue)
                return; // No en passant possible this turn

            // Direction depends on pawn color
            int direction = pawn.Color == PieceColor.White ? 1 : -1;

            // Check if pawn is on the correct rank for en passant
            int enPassantRank = pawn.Color == PieceColor.White ? 4 : 3;
            if (pawn.Position.Rank != enPassantRank)
                return;

            // Check if en passant target is diagonally adjacent
            BoardPosition targetSquare = enPassantTarget.Value;
            int fileDiff = targetSquare.File - pawn.Position.File;
            int rankDiff = targetSquare.Rank - pawn.Position.Rank;

            // En passant target must be one square forward and one file to the left or right
            if (rankDiff == direction && (fileDiff == -1 || fileDiff == 1))
            {
                // Find the enemy pawn that will be captured (it's on the same rank as our pawn)
                BoardPosition enemyPawnPos = new BoardPosition(targetSquare.File, pawn.Position.Rank);
                ChessPiece enemyPawn = board.GetPiece(enemyPawnPos);

                if (enemyPawn != null && enemyPawn is Pawn && enemyPawn.Color != pawn.Color)
                {
                    // Add en passant capture move
                    Move enPassantMove = Move.CreateEnPassant(pawn.Position, targetSquare, pawn, enemyPawn);
                    moves.Add(enPassantMove);
                }
            }
        }

        /// <summary>
        /// Gets all possible promotion piece types for Capablanca chess.
        /// Includes standard pieces plus Archbishop and Chancellor.
        /// </summary>
        public static PieceType[] GetPromotionOptions()
        {
            return new[]
            {
                PieceType.Queen,
                PieceType.Rook,
                PieceType.Bishop,
                PieceType.Knight,
                PieceType.Archbishop,
                PieceType.Chancellor
            };
        }

        /// <summary>
        /// Adds all promotion move variations for a pawn reaching the promotion rank.
        /// Creates a separate move for each possible promotion piece type.
        /// </summary>
        public void AddPromotionMoves(Pawn pawn, BoardPosition targetPos, List<Move> moves, ChessPiece capturedPiece = null)
        {
            int promotionRank = pawn.Color == PieceColor.White ? 7 : 0;

            if (targetPos.Rank == promotionRank)
            {
                // Add a move for each possible promotion type
                foreach (PieceType promotionType in GetPromotionOptions())
                {
                    moves.Add(Move.CreatePromotion(pawn.Position, targetPos, pawn, promotionType, capturedPiece));
                }
            }
        }
    }
}
