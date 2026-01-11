using System;
using UnityEngine;
using ChessGame.Core.Pieces;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.UI
{
    /// <summary>
    /// Handles pawn promotion piece selection.
    /// For now, uses keyboard input. Can be enhanced with UI buttons later.
    /// </summary>
    public class PromotionSelector : MonoBehaviour
    {
        /// <summary>
        /// Event fired when a promotion piece is selected.
        /// </summary>
        public event Action<PieceType> OnPromotionSelected;

        private bool isAwaitingSelection = false;
        private PieceColor promotingColor;

        /// <summary>
        /// Starts the promotion selection process.
        /// </summary>
        public void StartSelection(PieceColor color)
        {
            isAwaitingSelection = true;
            promotingColor = color;

            Debug.Log("=== PAWN PROMOTION ===");
            Debug.Log($"{color} pawn reached the promotion rank!");
            Debug.Log("Select promotion piece:");
            Debug.Log("Press Q - Queen");
            Debug.Log("Press R - Rook");
            Debug.Log("Press B - Bishop");
            Debug.Log("Press N - Knight");
            Debug.Log("Press A - Archbishop");
            Debug.Log("Press C - Chancellor");
        }

        /// <summary>
        /// Cancels the promotion selection.
        /// </summary>
        public void CancelSelection()
        {
            isAwaitingSelection = false;
        }

        /// <summary>
        /// Checks if currently awaiting promotion selection.
        /// </summary>
        public bool IsAwaitingSelection()
        {
            return isAwaitingSelection;
        }

        private void Update()
        {
            if (!isAwaitingSelection)
                return;

            // Check for keyboard input
            PieceType? selectedType = null;

            if (Input.GetKeyDown(KeyCode.Q))
                selectedType = PieceType.Queen;
            else if (Input.GetKeyDown(KeyCode.R))
                selectedType = PieceType.Rook;
            else if (Input.GetKeyDown(KeyCode.B))
                selectedType = PieceType.Bishop;
            else if (Input.GetKeyDown(KeyCode.N))
                selectedType = PieceType.Knight;
            else if (Input.GetKeyDown(KeyCode.A))
                selectedType = PieceType.Archbishop;
            else if (Input.GetKeyDown(KeyCode.C))
                selectedType = PieceType.Chancellor;

            if (selectedType.HasValue)
            {
                isAwaitingSelection = false;
                Debug.Log($"Selected promotion to: {selectedType.Value}");
                OnPromotionSelected?.Invoke(selectedType.Value);
            }
        }

        /// <summary>
        /// Gets the default promotion piece type (Queen).
        /// Used if no UI is available.
        /// </summary>
        public static PieceType GetDefaultPromotionType()
        {
            return PieceType.Queen;
        }
    }
}
