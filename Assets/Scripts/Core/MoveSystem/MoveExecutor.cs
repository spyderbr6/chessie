using System;
using UnityEngine;
using ChessGame.Core.Board;
using ChessGame.Core.Pieces;
using ChessGame.Core.GameLogic;

namespace ChessGame.Core.MoveSystem
{
    /// <summary>
    /// Executes chess moves by updating board state and piece positions.
    /// Handles captures, castling rights updates, and en passant state.
    /// </summary>
    public class MoveExecutor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ChessBoard board;
        [SerializeField] private BoardVisualizer boardVisualizer;
        [SerializeField] private TurnManager turnManager;

        /// <summary>
        /// Event fired when a move has been successfully executed.
        /// </summary>
        public event Action<Move> OnMoveExecuted;

        /// <summary>
        /// Event fired when a piece is captured.
        /// </summary>
        public event Action<ChessPiece> OnPieceCaptured;

        /// <summary>
        /// Executes a move on the board.
        /// Updates board state, piece positions, and game state.
        /// </summary>
        public void ExecuteMove(Move move)
        {
            if (move.MovingPiece == null)
            {
                Debug.LogError("Attempted to execute move with null moving piece!");
                return;
            }

            // Handle different move types
            if (move.IsCastling)
            {
                ExecuteCastling(move);
            }
            else if (move.IsEnPassant)
            {
                ExecuteEnPassant(move);
            }
            else if (move.IsPromotion)
            {
                ExecutePromotion(move);
            }
            else
            {
                ExecuteStandardMove(move);
            }

            // Update castling rights based on piece movement
            board.UpdateCastlingRights(move.From, move.MovingPiece);

            // Update en passant state
            UpdateEnPassantState(move);

            // Update half-move clock for 50-move rule
            UpdateHalfMoveClock(move);

            // Switch turns
            turnManager.EndTurn();

            // Fire event
            OnMoveExecuted?.Invoke(move);

            Debug.Log($"Executed move: {move}");
        }

        /// <summary>
        /// Executes a standard move (non-special).
        /// </summary>
        private void ExecuteStandardMove(Move move)
        {
            // Handle capture if present
            if (move.CapturedPiece != null)
            {
                CapturePiece(move.CapturedPiece, move.To);
            }

            // Move piece on board
            board.ClearPosition(move.From);
            board.SetPiece(move.To, move.MovingPiece);

            // Update piece state
            move.MovingPiece.MoveTo(move.To);

            // Update visual position
            UpdatePieceVisualPosition(move.MovingPiece, move.To);
        }

        /// <summary>
        /// Executes a castling move.
        /// Moves both king and rook.
        /// </summary>
        private void ExecuteCastling(Move move)
        {
            if (!move.RookMoveFrom.HasValue || !move.RookMoveTo.HasValue)
            {
                Debug.LogError("Castling move missing rook positions!");
                return;
            }

            // Move king
            board.ClearPosition(move.From);
            board.SetPiece(move.To, move.MovingPiece);
            move.MovingPiece.MoveTo(move.To);
            UpdatePieceVisualPosition(move.MovingPiece, move.To);

            // Move rook
            ChessPiece rook = board.GetPiece(move.RookMoveFrom.Value);
            if (rook != null)
            {
                board.ClearPosition(move.RookMoveFrom.Value);
                board.SetPiece(move.RookMoveTo.Value, rook);
                rook.MoveTo(move.RookMoveTo.Value);
                UpdatePieceVisualPosition(rook, move.RookMoveTo.Value);
            }
            else
            {
                Debug.LogError($"No rook found at {move.RookMoveFrom.Value} for castling!");
            }

            Debug.Log($"Castled: King {move.From.ToAlgebraic()}->{move.To.ToAlgebraic()}, " +
                     $"Rook {move.RookMoveFrom.Value.ToAlgebraic()}->{move.RookMoveTo.Value.ToAlgebraic()}");
        }

        /// <summary>
        /// Executes an en passant capture.
        /// Removes the captured pawn from a different square than the destination.
        /// </summary>
        private void ExecuteEnPassant(Move move)
        {
            // Move the capturing pawn
            board.ClearPosition(move.From);
            board.SetPiece(move.To, move.MovingPiece);
            move.MovingPiece.MoveTo(move.To);
            UpdatePieceVisualPosition(move.MovingPiece, move.To);

            // Capture the enemy pawn (which is on the same rank as the moving pawn)
            if (move.CapturedPiece != null)
            {
                BoardPosition capturedPawnPosition = move.CapturedPiece.Position;
                CapturePiece(move.CapturedPiece, capturedPawnPosition);
                Debug.Log($"En passant capture at {capturedPawnPosition.ToAlgebraic()}");
            }
        }

        /// <summary>
        /// Executes a pawn promotion.
        /// Replaces the pawn with the promoted piece.
        /// </summary>
        private void ExecutePromotion(Move move)
        {
            // Handle capture if present
            if (move.CapturedPiece != null)
            {
                CapturePiece(move.CapturedPiece, move.To);
            }

            // Remove the pawn from the board
            board.ClearPosition(move.From);

            // Create the promoted piece
            ChessPiece promotedPiece = CreatePromotedPiece(move.PromotionType, move.MovingPiece.Color, move.To);

            if (promotedPiece != null)
            {
                board.SetPiece(move.To, promotedPiece);
                UpdatePieceVisualPosition(promotedPiece, move.To);

                // Destroy the original pawn GameObject
                Destroy(move.MovingPiece.gameObject);

                Debug.Log($"Pawn promoted to {move.PromotionType} at {move.To.ToAlgebraic()}");
            }
            else
            {
                Debug.LogError($"Failed to create promoted piece of type {move.PromotionType}");
            }
        }

        /// <summary>
        /// Captures a piece by removing it from the board and destroying its GameObject.
        /// </summary>
        private void CapturePiece(ChessPiece piece, BoardPosition position)
        {
            // Fire capture event
            OnPieceCaptured?.Invoke(piece);

            // Call piece's OnCaptured method (for animations, etc.)
            piece.OnCaptured();

            // Update castling rights if rook captured
            if (piece is Rook)
            {
                board.HandleRookCapture(position);
            }

            // Remove from board
            board.RemovePiece(position);

            Debug.Log($"Captured {piece.Color} {piece.Type} at {position.ToAlgebraic()}");
        }

        /// <summary>
        /// Creates a promoted piece of the specified type.
        /// </summary>
        private ChessPiece CreatePromotedPiece(PieceType type, PieceColor color, BoardPosition position)
        {
            // Create a new GameObject for the promoted piece
            GameObject pieceObject = new GameObject($"{color}_{type}");
            pieceObject.transform.position = boardVisualizer.GetPieceWorldPosition(position);

            // Add SpriteRenderer
            SpriteRenderer renderer = pieceObject.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 7;

            // Add the appropriate piece component
            ChessPiece piece = type switch
            {
                PieceType.Queen => pieceObject.AddComponent<Queen>(),
                PieceType.Rook => pieceObject.AddComponent<Rook>(),
                PieceType.Bishop => pieceObject.AddComponent<Bishop>(),
                PieceType.Knight => pieceObject.AddComponent<Knight>(),
                PieceType.Archbishop => pieceObject.AddComponent<Archbishop>(),
                PieceType.Chancellor => pieceObject.AddComponent<Chancellor>(),
                _ => null
            };

            if (piece != null)
            {
                piece.Initialize(type, color, position);
                piece.HasMoved = true; // Promoted pieces have "moved"
            }

            return piece;
        }

        /// <summary>
        /// Updates the visual position of a piece GameObject.
        /// </summary>
        private void UpdatePieceVisualPosition(ChessPiece piece, BoardPosition position)
        {
            if (piece != null && boardVisualizer != null)
            {
                piece.transform.position = boardVisualizer.GetPieceWorldPosition(position);
            }
        }

        /// <summary>
        /// Updates the en passant target square based on the move.
        /// Set if a pawn moves two squares, cleared otherwise.
        /// </summary>
        private void UpdateEnPassantState(Move move)
        {
            // Clear previous en passant target
            board.ClearEnPassantTarget();

            // Set new en passant target if pawn moved two squares
            if (move.MovingPiece is Pawn)
            {
                int fileDiff = Math.Abs(move.To.File - move.From.File);
                int rankDiff = Math.Abs(move.To.Rank - move.From.Rank);

                if (fileDiff == 0 && rankDiff == 2)
                {
                    // Pawn moved two squares - set en passant target to the square it passed over
                    int direction = move.MovingPiece.Color == PieceColor.White ? 1 : -1;
                    BoardPosition enPassantTarget = new BoardPosition(move.From.File, move.From.Rank + direction);
                    board.SetEnPassantTarget(enPassantTarget);

                    Debug.Log($"En passant target set to {enPassantTarget.ToAlgebraic()}");
                }
            }
        }

        /// <summary>
        /// Updates the half-move clock for the 50-move rule.
        /// Resets on pawn moves or captures, increments otherwise.
        /// </summary>
        private void UpdateHalfMoveClock(Move move)
        {
            // Reset if pawn move or capture
            if (move.MovingPiece is Pawn || move.CapturedPiece != null)
            {
                turnManager.ResetHalfMoveClock();
            }
            else
            {
                turnManager.IncrementHalfMoveClock();
            }
        }
    }
}
