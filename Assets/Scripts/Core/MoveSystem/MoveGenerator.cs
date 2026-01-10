using System.Collections.Generic;
using UnityEngine;
using ChessGame.Core.Board;
using ChessGame.Core.Pieces;

namespace ChessGame.Core.MoveSystem
{
    /// <summary>
    /// Generates legal moves for chess pieces.
    /// Filters pseudo-legal moves to exclude those that would leave the king in check.
    /// </summary>
    public class MoveGenerator : MonoBehaviour
    {
        [SerializeField] private ChessBoard board;

        /// <summary>
        /// Gets all legal moves for a piece at the specified position.
        /// Returns empty list if no piece exists at that position.
        /// </summary>
        public List<Move> GetLegalMoves(BoardPosition position)
        {
            ChessPiece piece = board.GetPiece(position);
            if (piece == null)
                return new List<Move>();

            return GetLegalMovesForPiece(piece);
        }

        /// <summary>
        /// Gets all legal moves for the specified piece.
        /// Filters pseudo-legal moves to remove those that leave the king in check.
        /// </summary>
        public List<Move> GetLegalMovesForPiece(ChessPiece piece)
        {
            if (piece == null)
                return new List<Move>();

            // Get pseudo-legal moves (moves that follow piece rules but may leave king in check)
            List<Move> pseudoLegalMoves = piece.GetPseudoLegalMoves(board);

            // Filter out moves that would leave own king in check
            List<Move> legalMoves = new List<Move>();

            foreach (Move move in pseudoLegalMoves)
            {
                if (!WouldLeaveKingInCheck(move, piece.Color))
                {
                    legalMoves.Add(move);
                }
            }

            return legalMoves;
        }

        /// <summary>
        /// Gets all legal moves for all pieces of the specified color.
        /// </summary>
        public List<Move> GetAllLegalMoves(PieceColor color)
        {
            List<Move> allMoves = new List<Move>();

            // Iterate through all board positions
            for (int file = 0; file < 10; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    BoardPosition pos = new BoardPosition(file, rank);
                    ChessPiece piece = board.GetPiece(pos);

                    if (piece != null && piece.Color == color)
                    {
                        allMoves.AddRange(GetLegalMovesForPiece(piece));
                    }
                }
            }

            return allMoves;
        }

        /// <summary>
        /// Checks if a move would leave the moving player's king in check.
        /// Makes the move temporarily, checks for check, then undoes it.
        /// </summary>
        private bool WouldLeaveKingInCheck(Move move, PieceColor movingColor)
        {
            // Save state for undo
            BoardPosition from = move.From;
            BoardPosition to = move.To;
            ChessPiece movingPiece = board.GetPiece(from);
            ChessPiece capturedPiece = board.GetPiece(to);
            BoardPosition originalPosition = movingPiece.Position;
            bool originalHasMoved = movingPiece.HasMoved;

            // Make the move temporarily
            board.ClearPosition(from);
            board.SetPiece(to, movingPiece);
            movingPiece.HasMoved = true;

            // Check if king is in check after this move
            bool inCheck = IsKingInCheck(movingColor);

            // Undo the move
            board.SetPiece(from, movingPiece);
            movingPiece.Position = originalPosition;
            movingPiece.HasMoved = originalHasMoved;

            if (capturedPiece != null)
            {
                board.SetPiece(to, capturedPiece);
            }
            else
            {
                board.ClearPosition(to);
            }

            return inCheck;
        }

        /// <summary>
        /// Checks if the king of the specified color is currently in check.
        /// </summary>
        public bool IsKingInCheck(PieceColor kingColor)
        {
            King king = board.GetKing(kingColor);
            if (king == null)
            {
                Debug.LogError($"No {kingColor} king found on board!");
                return false;
            }

            BoardPosition kingPosition = king.Position;
            PieceColor enemyColor = kingColor == PieceColor.White ? PieceColor.Black : PieceColor.White;

            // Check if any enemy piece can attack the king's position
            return IsSquareAttackedBy(kingPosition, enemyColor);
        }

        /// <summary>
        /// Checks if a square is attacked by any piece of the specified color.
        /// Used for check detection and castling validation.
        /// </summary>
        public bool IsSquareAttackedBy(BoardPosition position, PieceColor attackerColor)
        {
            // Check all enemy pieces to see if any can attack this square
            for (int file = 0; file < 10; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    BoardPosition pos = new BoardPosition(file, rank);
                    ChessPiece piece = board.GetPiece(pos);

                    if (piece != null && piece.Color == attackerColor)
                    {
                        // Get pseudo-legal moves for this piece
                        List<Move> moves = piece.GetPseudoLegalMoves(board);

                        // Check if any move targets the position we're checking
                        foreach (Move move in moves)
                        {
                            if (move.To == position)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the specified color has any legal moves.
        /// Used for checkmate and stalemate detection.
        /// </summary>
        public bool HasLegalMoves(PieceColor color)
        {
            for (int file = 0; file < 10; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    BoardPosition pos = new BoardPosition(file, rank);
                    ChessPiece piece = board.GetPiece(pos);

                    if (piece != null && piece.Color == color)
                    {
                        List<Move> legalMoves = GetLegalMovesForPiece(piece);
                        if (legalMoves.Count > 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
