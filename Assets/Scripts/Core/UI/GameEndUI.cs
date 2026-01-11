using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChessGame.Core.GameLogic;
using ChessGame.Core.Pieces;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.UI
{
    /// <summary>
    /// Displays the game end screen with results and statistics.
    /// Shows when game ends (checkmate, stalemate, draw).
    /// </summary>
    public class GameEndUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject gameEndPanel;
        [SerializeField] private TMP_Text resultTitleText;
        [SerializeField] private TMP_Text resultDetailsText;
        [SerializeField] private TMP_Text gameStatsText;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button quitButton;

        [Header("Game References")]
        [SerializeField] private GameController gameController;
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private MoveHistory moveHistory;

        [Header("Colors")]
        [SerializeField] private Color winColor = Color.green;
        [SerializeField] private Color drawColor = Color.yellow;

        private void Start()
        {
            // Hide panel initially
            if (gameEndPanel != null)
            {
                gameEndPanel.SetActive(false);
            }

            // Set up button listeners
            if (newGameButton != null)
            {
                newGameButton.onClick.AddListener(OnNewGameClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitClicked);
            }
        }

        /// <summary>
        /// Shows the game end screen with results.
        /// </summary>
        public void ShowGameEnd(GameState endState, PieceColor? winner)
        {
            if (gameEndPanel != null)
            {
                gameEndPanel.SetActive(true);
            }

            UpdateResultTitle(endState, winner);
            UpdateResultDetails(endState, winner);
            UpdateGameStats();
        }

        /// <summary>
        /// Hides the game end screen.
        /// </summary>
        public void Hide()
        {
            if (gameEndPanel != null)
            {
                gameEndPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Updates the result title text.
        /// </summary>
        private void UpdateResultTitle(GameState endState, PieceColor? winner)
        {
            if (resultTitleText == null)
                return;

            string title = endState switch
            {
                GameState.Checkmate => $"{winner} WINS!",
                GameState.Stalemate => "STALEMATE",
                GameState.Draw => "DRAW",
                _ => "GAME OVER"
            };

            resultTitleText.text = title;

            // Color the title
            resultTitleText.color = endState == GameState.Checkmate ? winColor : drawColor;
        }

        /// <summary>
        /// Updates the result details text.
        /// </summary>
        private void UpdateResultDetails(GameState endState, PieceColor? winner)
        {
            if (resultDetailsText == null)
                return;

            string details = endState switch
            {
                GameState.Checkmate => GetCheckmateDetails(winner),
                GameState.Stalemate => "The game is drawn. No legal moves available but king is not in check.",
                GameState.Draw => GetDrawDetails(),
                _ => "The game has ended."
            };

            resultDetailsText.text = details;
        }

        /// <summary>
        /// Gets checkmate details message.
        /// </summary>
        private string GetCheckmateDetails(PieceColor? winner)
        {
            if (!winner.HasValue)
                return "Checkmate!";

            PieceColor loser = winner.Value == PieceColor.White ? PieceColor.Black : PieceColor.White;
            return $"{winner.Value} has checkmated {loser}!\nCongratulations!";
        }

        /// <summary>
        /// Gets draw details message.
        /// </summary>
        private string GetDrawDetails()
        {
            if (turnManager != null && turnManager.IsFiftyMoveRule())
            {
                return "The game is drawn by the 50-move rule.\nNo pawn moves or captures in 50 moves.";
            }

            return "The game is drawn.";
        }

        /// <summary>
        /// Updates the game statistics text.
        /// </summary>
        private void UpdateGameStats()
        {
            if (gameStatsText == null)
                return;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("=== GAME STATISTICS ===");

            if (turnManager != null)
            {
                sb.AppendLine($"Total Turns: {turnManager.TurnNumber}");
                sb.AppendLine($"Total Moves: {turnManager.TurnNumber * 2 - (turnManager.CurrentTurn == PieceColor.White ? 1 : 0)}");
            }

            if (moveHistory != null)
            {
                sb.AppendLine($"Moves Played: {moveHistory.MoveCount}");
                sb.AppendLine();
                sb.AppendLine("Move History:");
                sb.AppendLine(moveHistory.GetFormattedMoveList());
            }

            gameStatsText.text = sb.ToString();
        }

        /// <summary>
        /// Called when New Game button is clicked.
        /// </summary>
        private void OnNewGameClicked()
        {
            if (gameController != null)
            {
                gameController.NewGame();
            }

            Hide();
        }

        /// <summary>
        /// Called when Quit button is clicked.
        /// </summary>
        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
