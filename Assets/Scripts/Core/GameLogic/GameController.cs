using UnityEngine;
using ChessGame.Core.Board;
using ChessGame.Core.Pieces;
using ChessGame.Core.MoveSystem;
using ChessGame.Core.Input;
using ChessGame.Core.UI;

namespace ChessGame.Core.GameLogic
{
    /// <summary>
    /// Central game controller that coordinates all chess systems.
    /// Manages game initialization, move execution, and game state transitions.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        [Header("Core Systems")]
        [SerializeField] private ChessBoard board;
        [SerializeField] private BoardVisualizer boardVisualizer;
        [SerializeField] private BoardSetup boardSetup;

        [Header("Move System")]
        [SerializeField] private MoveGenerator moveGenerator;
        [SerializeField] private MoveExecutor moveExecutor;
        [SerializeField] private MoveHistory moveHistory;

        [Header("Input System")]
        [SerializeField] private ChessInputHandler inputHandler;
        [SerializeField] private PieceSelector pieceSelector;
        [SerializeField] private SquareHighlighter highlighter;

        [Header("Game Logic")]
        [SerializeField] private TurnManager turnManager;

        [Header("UI System")]
        [SerializeField] private GameStatusUI gameStatusUI;
        [SerializeField] private MoveHistoryUI moveHistoryUI;
        [SerializeField] private CapturedPiecesDisplay capturedPiecesDisplay;
        [SerializeField] private GameEndUI gameEndUI;

        [Header("Settings")]
        [SerializeField] private bool autoStartGame = true;

        private GameState currentState = GameState.NotStarted;

        /// <summary>
        /// The current game state.
        /// </summary>
        public GameState CurrentState => currentState;

        private void Start()
        {
            if (autoStartGame)
            {
                InitializeGame();
            }
        }

        private void OnEnable()
        {
            // Subscribe to input events
            if (inputHandler != null)
            {
                inputHandler.OnMoveSelected += HandleMoveSelected;
            }

            // Subscribe to move execution events
            if (moveExecutor != null)
            {
                moveExecutor.OnMoveExecuted += HandleMoveExecuted;
                moveExecutor.OnPieceCaptured += HandlePieceCaptured;
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from events
            if (inputHandler != null)
            {
                inputHandler.OnMoveSelected -= HandleMoveSelected;
            }

            if (moveExecutor != null)
            {
                moveExecutor.OnMoveExecuted -= HandleMoveExecuted;
                moveExecutor.OnPieceCaptured -= HandlePieceCaptured;
            }
        }

        /// <summary>
        /// Initializes a new game.
        /// </summary>
        public void InitializeGame()
        {
            Debug.Log("=== Initializing Capablanca Chess ===");

            // Clear any existing game state
            if (board != null)
            {
                board.ClearBoard();
            }

            if (moveHistory != null)
            {
                moveHistory.Clear();
            }

            // Clear UI displays
            if (moveHistoryUI != null)
            {
                moveHistoryUI.Clear();
            }

            if (capturedPiecesDisplay != null)
            {
                capturedPiecesDisplay.Clear();
            }

            if (gameEndUI != null)
            {
                gameEndUI.Hide();
            }

            // Create the board visualization
            if (boardVisualizer != null)
            {
                boardVisualizer.CreateBoard();
            }

            // Set up pieces in Capablanca starting position
            if (boardSetup != null)
            {
                boardSetup.SetupCapablancaPosition();
            }

            // Start turn manager
            if (turnManager != null)
            {
                turnManager.StartNewGame();
            }

            // Enable input
            if (inputHandler != null)
            {
                inputHandler.EnableInput();
            }

            // Set game state to in progress
            currentState = GameState.InProgress;

            // Update UI
            if (gameStatusUI != null)
            {
                gameStatusUI.UpdateUI();
            }

            Debug.Log($"Game started. {turnManager.CurrentTurn} to move.");
        }

        /// <summary>
        /// Handles when a player selects a move via input.
        /// </summary>
        private void HandleMoveSelected(Move move)
        {
            if (currentState != GameState.InProgress)
            {
                Debug.LogWarning("Cannot make move - game is not in progress");
                return;
            }

            if (move.MovingPiece == null)
            {
                Debug.LogError("Invalid move - no moving piece");
                return;
            }

            // Verify it's the correct player's turn
            if (move.MovingPiece.Color != turnManager.CurrentTurn)
            {
                Debug.LogWarning($"Cannot move {move.MovingPiece.Color} piece - it's {turnManager.CurrentTurn}'s turn");
                return;
            }

            // Execute the move
            ExecutePlayerMove(move);
        }

        /// <summary>
        /// Executes a player's move.
        /// </summary>
        private void ExecutePlayerMove(Move move)
        {
            Debug.Log($"Player move: {move}");

            // Clear piece selection
            if (pieceSelector != null)
            {
                pieceSelector.ClearSelection();
            }

            // Execute the move
            if (moveExecutor != null)
            {
                moveExecutor.ExecuteMove(move);
            }

            // Add to move history
            if (moveHistory != null)
            {
                moveHistory.AddMove(move);
            }
        }

        /// <summary>
        /// Called after a move has been executed.
        /// Checks for game-ending conditions.
        /// </summary>
        private void HandleMoveExecuted(Move move)
        {
            Debug.Log($"Move executed: {move} | Turn: {turnManager.TurnNumber}, {turnManager.CurrentTurn} to move");

            // Check for game-ending conditions
            CheckGameEndConditions();
        }

        /// <summary>
        /// Called when a piece is captured.
        /// </summary>
        private void HandlePieceCaptured(ChessPiece piece)
        {
            Debug.Log($"Piece captured: {piece.Color} {piece.Type}");
            // Could play capture sound, animation, etc.
        }

        /// <summary>
        /// Checks if the game has ended (checkmate, stalemate, draw).
        /// This is a placeholder - full implementation in Phase 8.
        /// </summary>
        private void CheckGameEndConditions()
        {
            // Check if current player is in check
            bool inCheck = moveGenerator.IsKingInCheck(turnManager.CurrentTurn);

            // Check if current player has legal moves
            bool hasLegalMoves = moveGenerator.HasLegalMoves(turnManager.CurrentTurn);

            if (!hasLegalMoves)
            {
                if (inCheck)
                {
                    // Checkmate
                    PieceColor winner = turnManager.GetOppositeColor(turnManager.CurrentTurn);
                    EndGame(GameState.Checkmate, winner);
                }
                else
                {
                    // Stalemate
                    EndGame(GameState.Stalemate, null);
                }
                return;
            }

            // Check for 50-move rule
            if (turnManager.IsFiftyMoveRule())
            {
                EndGame(GameState.Draw, null);
                return;
            }

            // Display check warning if in check
            if (inCheck)
            {
                Debug.Log($"{turnManager.CurrentTurn} is in CHECK!");
            }
        }

        /// <summary>
        /// Ends the game with the specified result.
        /// </summary>
        private void EndGame(GameState endState, PieceColor? winner)
        {
            currentState = endState;

            // Disable input
            if (inputHandler != null)
            {
                inputHandler.DisableInput();
            }

            // Log game result
            string resultMessage = endState switch
            {
                GameState.Checkmate => $"CHECKMATE! {winner} wins!",
                GameState.Stalemate => "STALEMATE! Game is a draw.",
                GameState.Draw => "DRAW by 50-move rule.",
                _ => "Game ended."
            };

            Debug.Log($"=== GAME OVER ===");
            Debug.Log(resultMessage);

            // Print move history
            if (moveHistory != null)
            {
                moveHistory.PrintMoveHistory();
            }

            // Show game end UI
            if (gameEndUI != null)
            {
                gameEndUI.ShowGameEnd(endState, winner);
            }

            // Update status UI
            if (gameStatusUI != null)
            {
                gameStatusUI.UpdateUI();
            }
        }

        /// <summary>
        /// Resets and starts a new game.
        /// </summary>
        public void NewGame()
        {
            InitializeGame();
        }

        /// <summary>
        /// Gets the current turn information as a string.
        /// </summary>
        public string GetTurnInfo()
        {
            return $"Turn {turnManager.TurnNumber} - {turnManager.CurrentTurn} to move";
        }
    }
}
