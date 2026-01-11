using System;
using System.Collections.Generic;
using UnityEngine;
using ChessGame.Core.Board;
using ChessGame.Core.Pieces;
using ChessGame.Core.MoveSystem;
using ChessGame.Core.GameLogic;

namespace ChessGame.Core.Input
{
    /// <summary>
    /// Manages piece selection and deselection logic.
    /// Ensures only the current player's pieces can be selected.
    /// </summary>
    public class PieceSelector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ChessBoard board;
        [SerializeField] private MoveGenerator moveGenerator;
        [SerializeField] private SquareHighlighter highlighter;
        [SerializeField] private TurnManager turnManager;

        /// <summary>
        /// Event fired when a piece is selected.
        /// </summary>
        public event Action<ChessPiece, List<Move>> OnPieceSelected;

        /// <summary>
        /// Event fired when a piece is deselected.
        /// </summary>
        public event Action OnPieceDeselected;

        /// <summary>
        /// The currently selected piece, if any.
        /// </summary>
        public ChessPiece SelectedPiece { get; private set; }

        /// <summary>
        /// Legal moves for the currently selected piece.
        /// </summary>
        public List<Move> SelectedPieceLegalMoves { get; private set; }

        /// <summary>
        /// Attempts to select a piece at the specified position.
        /// Returns true if a piece was successfully selected.
        /// </summary>
        public bool TrySelectPiece(BoardPosition position)
        {
            ChessPiece piece = board.GetPiece(position);

            // Can't select empty square
            if (piece == null)
            {
                return false;
            }

            // Can't select opponent's pieces
            if (piece.Color != turnManager.CurrentTurn)
            {
                Debug.Log($"Cannot select {piece.Color} piece - it's {turnManager.CurrentTurn}'s turn");
                return false;
            }

            // Get legal moves for this piece
            List<Move> legalMoves = moveGenerator.GetLegalMovesForPiece(piece);

            // Can't select piece with no legal moves
            if (legalMoves.Count == 0)
            {
                Debug.Log($"{piece} has no legal moves");
                return false;
            }

            // Selection successful
            SelectPiece(piece, legalMoves);
            return true;
        }

        /// <summary>
        /// Selects a piece and highlights it along with its legal moves.
        /// </summary>
        private void SelectPiece(ChessPiece piece, List<Move> legalMoves)
        {
            SelectedPiece = piece;
            SelectedPieceLegalMoves = legalMoves;

            // Highlight selected square
            highlighter.HighlightSelectedSquare(piece.Position);

            // Highlight legal move squares
            highlighter.HighlightLegalMoves(legalMoves);

            // Fire event
            OnPieceSelected?.Invoke(piece, legalMoves);

            Debug.Log($"Selected {piece} with {legalMoves.Count} legal moves");
        }

        /// <summary>
        /// Deselects the currently selected piece.
        /// </summary>
        public void DeselectPiece()
        {
            if (SelectedPiece == null)
                return;

            Debug.Log($"Deselected {SelectedPiece}");

            SelectedPiece = null;
            SelectedPieceLegalMoves = null;

            // Clear highlights
            highlighter.ClearAllHighlights();

            // Fire event
            OnPieceDeselected?.Invoke();
        }

        /// <summary>
        /// Checks if there is a currently selected piece.
        /// </summary>
        public bool HasSelection()
        {
            return SelectedPiece != null;
        }

        /// <summary>
        /// Attempts to get the legal move to the specified position, if one exists.
        /// Returns null if no legal move to that position.
        /// </summary>
        public Move GetLegalMoveToPosition(BoardPosition targetPosition)
        {
            if (SelectedPieceLegalMoves == null)
                return null;

            foreach (Move move in SelectedPieceLegalMoves)
            {
                if (move.To == targetPosition)
                {
                    return move;
                }
            }

            return null;
        }

        /// <summary>
        /// Handles a click on a square. Either selects a piece, moves a piece, or deselects.
        /// Returns true if a move was executed.
        /// </summary>
        public bool HandleSquareClick(BoardPosition position)
        {
            if (!HasSelection())
            {
                // No piece selected - try to select piece at this position
                TrySelectPiece(position);
                return false;
            }
            else
            {
                // Piece already selected
                // Check if clicking on a legal move destination
                Move move = GetLegalMoveToPosition(position);

                if (move != null)
                {
                    // Valid move destination - but don't execute here
                    // Let the input handler pass the move to the game logic
                    return true;
                }
                else
                {
                    // Not a legal move destination
                    // Check if clicking another friendly piece to switch selection
                    ChessPiece clickedPiece = board.GetPiece(position);

                    if (clickedPiece != null && clickedPiece.Color == turnManager.CurrentTurn)
                    {
                        // Clicked a different friendly piece - switch selection
                        DeselectPiece();
                        TrySelectPiece(position);
                    }
                    else
                    {
                        // Clicked an invalid square - deselect
                        DeselectPiece();
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Clears the selection without firing events (used after moves).
        /// </summary>
        public void ClearSelection()
        {
            SelectedPiece = null;
            SelectedPieceLegalMoves = null;
            highlighter.ClearAllHighlights();
        }
    }
}
