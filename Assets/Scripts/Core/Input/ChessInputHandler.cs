using UnityEngine;
using UnityEngine.InputSystem;
using ChessGame.Core.Board;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.Input
{
    /// <summary>
    /// Handles mouse input for the chess game.
    /// Converts mouse clicks to board positions and coordinates with PieceSelector.
    /// </summary>
    public class ChessInputHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private PieceSelector pieceSelector;

        [Header("Input Actions")]
        [SerializeField] private InputActionReference clickAction;

        /// <summary>
        /// Event fired when a valid move is selected by the player.
        /// The game logic will subscribe to this to execute moves.
        /// </summary>
        public System.Action<Move> OnMoveSelected;

        private void Awake()
        {
            // Get main camera if not assigned
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    Debug.LogError("ChessInputHandler: No main camera found!");
                }
            }
        }

        private void OnEnable()
        {
            if (clickAction != null)
            {
                clickAction.action.Enable();
                clickAction.action.performed += OnClickPerformed;
            }
        }

        private void OnDisable()
        {
            if (clickAction != null)
            {
                clickAction.action.performed -= OnClickPerformed;
                clickAction.action.Disable();
            }
        }

        /// <summary>
        /// Called when the click input action is performed.
        /// </summary>
        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            HandleClick();
        }

        /// <summary>
        /// Alternative update method for mouse input without Input Actions.
        /// Enable this if Input Actions are not configured.
        /// </summary>
        private void Update()
        {
            // Fallback: Handle mouse input directly if no Input Action is configured
            if (clickAction == null && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleClick();
            }
        }

        /// <summary>
        /// Handles a mouse click and converts it to a board interaction.
        /// </summary>
        private void HandleClick()
        {
            Vector2 mousePosition;

            // Get mouse position from new Input System or legacy
            if (Mouse.current != null)
            {
                mousePosition = Mouse.current.position.ReadValue();
            }
            else
            {
                // Fallback to legacy input
                mousePosition = UnityEngine.Input.mousePosition;
            }

            // Raycast to detect which square was clicked
            BoardPosition? clickedPosition = GetBoardPositionFromMouse(mousePosition);

            if (clickedPosition.HasValue)
            {
                HandleSquareClick(clickedPosition.Value);
            }
        }

        /// <summary>
        /// Converts mouse screen position to a board position via raycasting.
        /// Returns null if no board square was clicked.
        /// </summary>
        private BoardPosition? GetBoardPositionFromMouse(Vector2 screenPosition)
        {
            if (mainCamera == null)
                return null;

            // Raycast from mouse position
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                // Check if we hit a board square
                BoardSquare boardSquare = hit.collider.GetComponent<BoardSquare>();
                if (boardSquare != null)
                {
                    return boardSquare.Position;
                }
            }

            return null;
        }

        /// <summary>
        /// Handles a click on a specific board square.
        /// Coordinates with PieceSelector to select pieces or execute moves.
        /// </summary>
        private void HandleSquareClick(BoardPosition position)
        {
            Debug.Log($"Clicked square: {position.ToAlgebraic()}");

            // Let PieceSelector handle the logic
            bool moveSelected = pieceSelector.HandleSquareClick(position);

            if (moveSelected)
            {
                // A valid move was selected - get the move and fire event
                Move move = pieceSelector.GetLegalMoveToPosition(position);
                if (move != null)
                {
                    Debug.Log($"Move selected: {move}");
                    OnMoveSelected?.Invoke(move);
                }
            }
        }

        /// <summary>
        /// Enables input handling.
        /// </summary>
        public void EnableInput()
        {
            enabled = true;
        }

        /// <summary>
        /// Disables input handling (useful during animations or AI turns).
        /// </summary>
        public void DisableInput()
        {
            enabled = false;
            pieceSelector.DeselectPiece(); // Clear selection when disabling input
        }
    }
}
