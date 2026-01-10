using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.UI
{
    /// <summary>
    /// Displays the move history in a scrollable text area.
    /// Updates automatically as moves are played.
    /// </summary>
    public class MoveHistoryUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text moveHistoryText;
        [SerializeField] private ScrollRect scrollRect;

        [Header("Game References")]
        [SerializeField] private MoveHistory moveHistory;
        [SerializeField] private MoveExecutor moveExecutor;

        [Header("Settings")]
        [SerializeField] private bool autoScroll = true;
        [SerializeField] private int maxVisibleMoves = 50; // Trim old moves to prevent performance issues

        private void OnEnable()
        {
            if (moveExecutor != null)
            {
                moveExecutor.OnMoveExecuted += HandleMoveExecuted;
            }
        }

        private void OnDisable()
        {
            if (moveExecutor != null)
            {
                moveExecutor.OnMoveExecuted -= HandleMoveExecuted;
            }
        }

        private void Start()
        {
            UpdateMoveHistoryDisplay();
        }

        /// <summary>
        /// Called when a move is executed.
        /// </summary>
        private void HandleMoveExecuted(Move move)
        {
            UpdateMoveHistoryDisplay();

            if (autoScroll)
            {
                ScrollToBottom();
            }
        }

        /// <summary>
        /// Updates the move history text display.
        /// </summary>
        public void UpdateMoveHistoryDisplay()
        {
            if (moveHistoryText == null || moveHistory == null)
                return;

            if (moveHistory.MoveCount == 0)
            {
                moveHistoryText.text = "No moves yet";
                return;
            }

            // Get formatted move list
            string formattedMoves = moveHistory.GetFormattedMoveList();

            // Optionally trim to recent moves only for performance
            if (maxVisibleMoves > 0 && moveHistory.MoveCount > maxVisibleMoves * 2)
            {
                // Show only the last N moves
                int startIndex = moveHistory.MoveCount - (maxVisibleMoves * 2);
                string recentMoves = GetRecentMovesFormatted(startIndex);
                moveHistoryText.text = $"... (showing recent {maxVisibleMoves} moves)\n{recentMoves}";
            }
            else
            {
                moveHistoryText.text = formattedMoves;
            }
        }

        /// <summary>
        /// Gets formatted move list starting from a specific move index.
        /// </summary>
        private string GetRecentMovesFormatted(int startIndex)
        {
            if (moveHistory == null || startIndex >= moveHistory.MoveCount)
                return "";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = startIndex; i < moveHistory.MoveCount; i += 2)
            {
                int moveNumber = (i / 2) + 1;
                Move? whiteMove = moveHistory.GetMove(i);

                if (whiteMove.HasValue)
                {
                    string whiteMoveNotation = $"{whiteMove.Value.From.ToAlgebraic()}-{whiteMove.Value.To.ToAlgebraic()}";

                    if (i + 1 < moveHistory.MoveCount)
                    {
                        Move? blackMove = moveHistory.GetMove(i + 1);
                        if (blackMove.HasValue)
                        {
                            string blackMoveNotation = $"{blackMove.Value.From.ToAlgebraic()}-{blackMove.Value.To.ToAlgebraic()}";
                            sb.AppendLine($"{moveNumber}. {whiteMoveNotation} {blackMoveNotation}");
                        }
                    }
                    else
                    {
                        sb.AppendLine($"{moveNumber}. {whiteMoveNotation}");
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Scrolls the scroll view to the bottom to show the most recent move.
        /// </summary>
        private void ScrollToBottom()
        {
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        /// <summary>
        /// Clears the move history display.
        /// </summary>
        public void Clear()
        {
            if (moveHistoryText != null)
            {
                moveHistoryText.text = "No moves yet";
            }
        }

        /// <summary>
        /// Exports the move history to a string (for saving/copying).
        /// </summary>
        public string ExportMoveHistory()
        {
            if (moveHistory == null)
                return "";

            return moveHistory.GetFormattedMoveList();
        }
    }
}
