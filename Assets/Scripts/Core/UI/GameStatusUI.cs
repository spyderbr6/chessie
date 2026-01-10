using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChessGame.Core.GameLogic;
using ChessGame.Core.Pieces;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.UI
{
    /// <summary>
    /// Displays game status information: current turn, check status, game state.
    /// Connect to Unity UI Text/TextMeshPro components in inspector.
    /// </summary>
    public class GameStatusUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text turnText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text checkWarningText;

        [Header("Game References")]
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private GameController gameController;
        [SerializeField] private MoveGenerator moveGenerator;

        [Header("Settings")]
        [SerializeField] private Color whiteColor = Color.white;
        [SerializeField] private Color blackColor = new Color(0.2f, 0.2f, 0.2f);
        [SerializeField] private Color checkWarningColor = Color.red;

        private void OnEnable()
        {
            if (turnManager != null)
            {
                turnManager.OnTurnChanged += HandleTurnChanged;
            }
        }

        private void OnDisable()
        {
            if (turnManager != null)
            {
                turnManager.OnTurnChanged -= HandleTurnChanged;
            }
        }

        private void Start()
        {
            UpdateUI();
        }

        /// <summary>
        /// Called when the turn changes.
        /// </summary>
        private void HandleTurnChanged(PieceColor currentTurn)
        {
            UpdateUI();
        }

        /// <summary>
        /// Updates all UI elements with current game state.
        /// </summary>
        public void UpdateUI()
        {
            UpdateTurnDisplay();
            UpdateStatusDisplay();
            UpdateCheckWarning();
        }

        /// <summary>
        /// Updates the turn display text.
        /// </summary>
        private void UpdateTurnDisplay()
        {
            if (turnText == null || turnManager == null)
                return;

            PieceColor currentTurn = turnManager.CurrentTurn;
            int turnNumber = turnManager.TurnNumber;

            string turnInfo = $"Turn {turnNumber} - {currentTurn} to move";
            turnText.text = turnInfo;

            // Color the text based on whose turn it is
            turnText.color = currentTurn == PieceColor.White ? whiteColor : blackColor;
        }

        /// <summary>
        /// Updates the game status text.
        /// </summary>
        private void UpdateStatusDisplay()
        {
            if (statusText == null || gameController == null)
                return;

            GameState state = gameController.CurrentState;

            string statusInfo = state switch
            {
                GameState.NotStarted => "Game not started",
                GameState.InProgress => "Game in progress",
                GameState.AwaitingPromotion => "Waiting for promotion...",
                GameState.Checkmate => GetCheckmateMessage(),
                GameState.Stalemate => "Game drawn by stalemate",
                GameState.Draw => "Game drawn (50-move rule)",
                _ => ""
            };

            statusText.text = statusInfo;
        }

        /// <summary>
        /// Updates the check warning display.
        /// </summary>
        private void UpdateCheckWarning()
        {
            if (checkWarningText == null || moveGenerator == null || turnManager == null)
                return;

            // Check if current player is in check
            bool inCheck = moveGenerator.IsKingInCheck(turnManager.CurrentTurn);

            if (inCheck && gameController.CurrentState == GameState.InProgress)
            {
                checkWarningText.text = $"{turnManager.CurrentTurn} is in CHECK!";
                checkWarningText.color = checkWarningColor;
                checkWarningText.gameObject.SetActive(true);
            }
            else
            {
                checkWarningText.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Gets the checkmate message with winner information.
        /// </summary>
        private string GetCheckmateMessage()
        {
            if (turnManager == null)
                return "Checkmate!";

            // The current player is the one who was just checkmated
            PieceColor loser = turnManager.CurrentTurn;
            PieceColor winner = turnManager.GetOppositeColor(loser);

            return $"CHECKMATE! {winner} wins!";
        }

        /// <summary>
        /// Forces an immediate UI update. Useful when game state changes.
        /// </summary>
        public void ForceUpdate()
        {
            UpdateUI();
        }
    }
}
