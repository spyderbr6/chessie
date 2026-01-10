using UnityEngine;
using ChessGame.Core.Pieces;

namespace ChessGame.Core.Board
{
    /// <summary>
    /// Manages the chess board state including piece positions, castling rights, and en passant.
    /// </summary>
    public class ChessBoard : MonoBehaviour
    {
        private ChessPiece[,] pieces = new ChessPiece[10, 8]; // [file, rank]

        // Castling rights
        private bool whiteKingSideCastle = true;
        private bool whiteQueenSideCastle = true;
        private bool blackKingSideCastle = true;
        private bool blackQueenSideCastle = true;

        // En passant target square
        private BoardPosition? enPassantTarget = null;

        // King references for performance (checking for check)
        private King whiteKing;
        private King blackKing;

        /// <summary>
        /// Gets the piece at the specified position. Returns null if empty or invalid position.
        /// </summary>
        public ChessPiece GetPiece(BoardPosition position)
        {
            if (!position.IsValid())
                return null;

            return pieces[position.File, position.Rank];
        }

        /// <summary>
        /// Sets a piece at the specified position. Updates the piece's position property.
        /// </summary>
        public void SetPiece(BoardPosition position, ChessPiece piece)
        {
            if (!position.IsValid())
            {
                Debug.LogError($"Attempted to set piece at invalid position: {position}");
                return;
            }

            pieces[position.File, position.Rank] = piece;

            if (piece != null)
            {
                piece.Position = position;

                // Track king references
                if (piece is King king)
                {
                    if (king.Color == PieceColor.White)
                        whiteKing = king;
                    else
                        blackKing = king;
                }
            }
        }

        /// <summary>
        /// Removes the piece at the specified position and destroys its GameObject.
        /// </summary>
        public void RemovePiece(BoardPosition position)
        {
            if (!position.IsValid())
                return;

            ChessPiece piece = pieces[position.File, position.Rank];
            if (piece != null)
            {
                pieces[position.File, position.Rank] = null;
                Destroy(piece.gameObject);
            }
        }

        /// <summary>
        /// Clears the piece reference at the specified position without destroying the GameObject.
        /// Used when moving pieces.
        /// </summary>
        public void ClearPosition(BoardPosition position)
        {
            if (!position.IsValid())
                return;

            pieces[position.File, position.Rank] = null;
        }

        /// <summary>
        /// Gets the King of the specified color.
        /// </summary>
        public King GetKing(PieceColor color)
        {
            return color == PieceColor.White ? whiteKing : blackKing;
        }

        /// <summary>
        /// Checks if a position on the board is empty.
        /// </summary>
        public bool IsEmpty(BoardPosition position)
        {
            return position.IsValid() && GetPiece(position) == null;
        }

        /// <summary>
        /// Checks if a position contains an enemy piece relative to the given color.
        /// </summary>
        public bool IsEnemyPiece(BoardPosition position, PieceColor friendlyColor)
        {
            ChessPiece piece = GetPiece(position);
            return piece != null && piece.Color != friendlyColor;
        }

        // Castling rights management

        /// <summary>
        /// Checks if castling is still allowed for the given color and side.
        /// </summary>
        public bool CanCastle(PieceColor color, bool kingSide)
        {
            if (color == PieceColor.White)
                return kingSide ? whiteKingSideCastle : whiteQueenSideCastle;
            else
                return kingSide ? blackKingSideCastle : blackQueenSideCastle;
        }

        /// <summary>
        /// Updates castling rights based on a piece move or capture.
        /// Disables castling when King or Rooks move.
        /// </summary>
        public void UpdateCastlingRights(BoardPosition from, ChessPiece movedPiece)
        {
            // Disable castling if King moves
            if (movedPiece is King)
            {
                if (movedPiece.Color == PieceColor.White)
                {
                    whiteKingSideCastle = false;
                    whiteQueenSideCastle = false;
                }
                else
                {
                    blackKingSideCastle = false;
                    blackQueenSideCastle = false;
                }
            }

            // Disable castling if Rook moves from starting position
            if (movedPiece is Rook)
            {
                if (movedPiece.Color == PieceColor.White && from.Rank == 0)
                {
                    if (from.File == 0) // Queen-side rook (a1)
                        whiteQueenSideCastle = false;
                    else if (from.File == 9) // King-side rook (j1)
                        whiteKingSideCastle = false;
                }
                else if (movedPiece.Color == PieceColor.Black && from.Rank == 7)
                {
                    if (from.File == 0) // Queen-side rook (a8)
                        blackQueenSideCastle = false;
                    else if (from.File == 9) // King-side rook (j8)
                        blackKingSideCastle = false;
                }
            }
        }

        /// <summary>
        /// Disables castling rights when a rook is captured.
        /// </summary>
        public void HandleRookCapture(BoardPosition capturePosition)
        {
            // White rooks
            if (capturePosition.Rank == 0)
            {
                if (capturePosition.File == 0)
                    whiteQueenSideCastle = false;
                else if (capturePosition.File == 9)
                    whiteKingSideCastle = false;
            }
            // Black rooks
            else if (capturePosition.Rank == 7)
            {
                if (capturePosition.File == 0)
                    blackQueenSideCastle = false;
                else if (capturePosition.File == 9)
                    blackKingSideCastle = false;
            }
        }

        // En passant management

        /// <summary>
        /// Sets the en passant target square (the square behind a pawn that just moved 2 squares).
        /// </summary>
        public void SetEnPassantTarget(BoardPosition? target)
        {
            enPassantTarget = target;
        }

        /// <summary>
        /// Gets the current en passant target square, if any.
        /// </summary>
        public BoardPosition? GetEnPassantTarget()
        {
            return enPassantTarget;
        }

        /// <summary>
        /// Clears the en passant target (should be called after each move).
        /// </summary>
        public void ClearEnPassantTarget()
        {
            enPassantTarget = null;
        }

        /// <summary>
        /// Resets all castling rights (used when setting up a new game).
        /// </summary>
        public void ResetCastlingRights()
        {
            whiteKingSideCastle = true;
            whiteQueenSideCastle = true;
            blackKingSideCastle = true;
            blackQueenSideCastle = true;
        }

        /// <summary>
        /// Clears the entire board (removes all pieces).
        /// </summary>
        public void ClearBoard()
        {
            for (int file = 0; file < 10; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    BoardPosition pos = new BoardPosition(file, rank);
                    RemovePiece(pos);
                }
            }

            whiteKing = null;
            blackKing = null;
            ClearEnPassantTarget();
            ResetCastlingRights();
        }
    }
}
