using UnityEngine;

namespace ChessGame.Core.Board
{
    /// <summary>
    /// Renders the 10x8 chess board with alternating light and dark squares.
    /// </summary>
    public class BoardVisualizer : MonoBehaviour
    {
        [Header("Board Settings")]
        [SerializeField] private float squareSize = 1f;
        [SerializeField] private Vector2 boardOffset = new Vector2(-4.5f, -3.5f);

        [Header("Square Colors")]
        [SerializeField] private Color lightSquareColor = new Color(0.94f, 0.85f, 0.71f); // #F0D9B5
        [SerializeField] private Color darkSquareColor = new Color(0.71f, 0.53f, 0.39f);  // #B58863

        [Header("Prefabs")]
        [SerializeField] private GameObject squarePrefab;

        private GameObject[,] squares = new GameObject[10, 8];
        private SpriteRenderer[,] squareRenderers = new GameObject[10, 8];

        private void Awake()
        {
            // If no prefab is provided, create a simple square prefab at runtime
            if (squarePrefab == null)
            {
                squarePrefab = CreateDefaultSquarePrefab();
            }
        }

        /// <summary>
        /// Creates the visual board with all squares.
        /// </summary>
        public void CreateBoard()
        {
            for (int file = 0; file < 10; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    BoardPosition position = new BoardPosition(file, rank);
                    CreateSquare(position);
                }
            }
        }

        /// <summary>
        /// Creates a single board square at the specified position.
        /// </summary>
        private void CreateSquare(BoardPosition position)
        {
            Vector3 worldPos = BoardToWorldPosition(position);
            GameObject square = Instantiate(squarePrefab, worldPos, Quaternion.identity, transform);
            square.name = $"Square_{position.ToAlgebraic()}";

            // Set square color based on position
            SpriteRenderer renderer = square.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                bool isLightSquare = (position.File + position.Rank) % 2 == 0;
                renderer.color = isLightSquare ? lightSquareColor : darkSquareColor;
                renderer.sortingOrder = 0; // Board is at the bottom layer
            }

            // Add BoxCollider2D for raycasting
            BoxCollider2D collider = square.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = square.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(squareSize, squareSize);
            }

            // Add BoardSquare component to store position data
            BoardSquare boardSquare = square.GetComponent<BoardSquare>();
            if (boardSquare == null)
            {
                boardSquare = square.AddComponent<BoardSquare>();
            }
            boardSquare.Position = position;

            squares[position.File, position.Rank] = square;
        }

        /// <summary>
        /// Converts a board position to world space coordinates.
        /// </summary>
        public Vector3 BoardToWorldPosition(BoardPosition position)
        {
            return new Vector3(
                boardOffset.x + position.File * squareSize,
                boardOffset.y + position.Rank * squareSize,
                0f
            );
        }

        /// <summary>
        /// Gets the world position for a piece at the specified board position.
        /// Pieces are positioned slightly in front of the board (z = -1).
        /// </summary>
        public Vector3 GetPieceWorldPosition(BoardPosition position)
        {
            Vector3 pos = BoardToWorldPosition(position);
            pos.z = -1f; // Pieces in front of board
            return pos;
        }

        /// <summary>
        /// Creates a default square prefab with a white sprite.
        /// </summary>
        private GameObject CreateDefaultSquarePrefab()
        {
            GameObject prefab = new GameObject("Square");

            // Create a simple white square sprite
            SpriteRenderer renderer = prefab.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateSquareSprite();
            renderer.sortingLayerName = "Default";
            renderer.sortingOrder = 0;

            // Add collider for clicking
            BoxCollider2D collider = prefab.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(squareSize, squareSize);

            return prefab;
        }

        /// <summary>
        /// Creates a simple square sprite texture.
        /// </summary>
        private Sprite CreateSquareSprite()
        {
            int textureSize = 64;
            Texture2D texture = new Texture2D(textureSize, textureSize);

            // Fill with white
            Color[] pixels = new Color[textureSize * textureSize];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }

            texture.SetPixels(pixels);
            texture.Apply();
            texture.filterMode = FilterMode.Point;

            return Sprite.Create(
                texture,
                new Rect(0, 0, textureSize, textureSize),
                new Vector2(0.5f, 0.5f),
                textureSize / squareSize
            );
        }

        /// <summary>
        /// Clears all board squares.
        /// </summary>
        public void ClearBoard()
        {
            for (int file = 0; file < 10; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    if (squares[file, rank] != null)
                    {
                        Destroy(squares[file, rank]);
                        squares[file, rank] = null;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            ClearBoard();
        }

#if UNITY_EDITOR
        // Visualize the board in the editor
        private void OnDrawGizmos()
        {
            for (int file = 0; file < 10; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    BoardPosition pos = new BoardPosition(file, rank);
                    Vector3 worldPos = BoardToWorldPosition(pos);

                    bool isLightSquare = (file + rank) % 2 == 0;
                    Gizmos.color = isLightSquare ? lightSquareColor : darkSquareColor;
                    Gizmos.DrawCube(worldPos, new Vector3(squareSize * 0.95f, squareSize * 0.95f, 0.1f));
                }
            }
        }
#endif
    }
}
