using UnityEngine;
using ChessGame.Core.Board;
using ChessGame.Core.GameLogic;

namespace ChessGame.Core.GameLogic
{
    /// <summary>
    /// Simple script to initialize the chess game on startup.
    /// Attach this to your ChessManager GameObject.
    /// </summary>
    public class GameInitializer : MonoBehaviour
    {
        [Header("Required Components")]
        [SerializeField] private BoardVisualizer boardVisualizer;
        [SerializeField] private BoardSetup boardSetup;
        [SerializeField] private TurnManager turnManager;

        private void Start()
        {
            // Initialize the game in correct order
            InitializeGame();
        }

        /// <summary>
        /// Sets up the chess board and pieces for a new game.
        /// </summary>
        private void InitializeGame()
        {
            // 1. Create the visual board (10x8 grid)
            boardVisualizer.CreateBoard();

            // 2. Setup pieces in Capablanca starting position
            boardSetup.SetupCapablancaPosition();

            // 3. Start the game with White to move
            turnManager.StartNewGame();

            Debug.Log("Capablanca Chess game initialized! White to move.");
        }
    }
}
