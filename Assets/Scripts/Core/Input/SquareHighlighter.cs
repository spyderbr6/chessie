using System.Collections.Generic;
using UnityEngine;
using ChessGame.Core.Board;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.Input
{
    /// <summary>
    /// Manages visual highlighting of board squares for selected pieces and legal moves.
    /// Uses sprite overlays for performance and visual clarity.
    /// </summary>
    public class SquareHighlighter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BoardVisualizer boardVisualizer;

        [Header("Highlight Colors")]
        [SerializeField] private Color selectedSquareColor = new Color(0.2f, 0.8f, 0.2f, 0.5f); // Semi-transparent green
        [SerializeField] private Color legalMoveColor = new Color(0.3f, 0.3f, 0.3f, 0.4f);      // Semi-transparent gray
        [SerializeField] private Color captureColor = new Color(0.9f, 0.2f, 0.2f, 0.5f);        // Semi-transparent red

        [Header("Settings")]
        [SerializeField] private float highlightSize = 1f;

        // Object pooling for highlights
        private List<GameObject> activeHighlights = new List<GameObject>();
        private Queue<GameObject> highlightPool = new Queue<GameObject>();
        private const int INITIAL_POOL_SIZE = 40; // Max possible legal moves is typically less

        private GameObject selectedHighlight;

        private void Awake()
        {
            // Pre-create highlight objects for object pooling
            for (int i = 0; i < INITIAL_POOL_SIZE; i++)
            {
                GameObject highlight = CreateHighlight();
                highlight.SetActive(false);
                highlightPool.Enqueue(highlight);
            }
        }

        /// <summary>
        /// Creates a highlight GameObject with a sprite renderer.
        /// </summary>
        private GameObject CreateHighlight()
        {
            GameObject highlight = new GameObject("Highlight");
            highlight.transform.SetParent(transform);

            SpriteRenderer renderer = highlight.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateCircleSprite();
            renderer.sortingOrder = 5; // Above board (0) but below pieces (7)

            return highlight;
        }

        /// <summary>
        /// Creates a circular sprite for the highlight indicator.
        /// </summary>
        private Sprite CreateCircleSprite()
        {
            int size = 64;
            Texture2D texture = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];

            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 pos = new Vector2(x, y);
                    float distance = Vector2.Distance(pos, center);

                    if (distance <= radius)
                    {
                        pixels[y * size + x] = Color.white;
                    }
                    else
                    {
                        pixels[y * size + x] = Color.clear;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            texture.filterMode = FilterMode.Bilinear;

            return Sprite.Create(
                texture,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                size / highlightSize
            );
        }

        /// <summary>
        /// Highlights the selected square with a distinct color.
        /// </summary>
        public void HighlightSelectedSquare(BoardPosition position)
        {
            ClearSelectedHighlight();

            selectedHighlight = GetHighlightFromPool();
            Vector3 worldPos = boardVisualizer.BoardToWorldPosition(position);
            worldPos.z = -0.5f; // Between board and pieces

            selectedHighlight.transform.position = worldPos;
            selectedHighlight.GetComponent<SpriteRenderer>().color = selectedSquareColor;
            selectedHighlight.SetActive(true);
        }

        /// <summary>
        /// Highlights all squares that are legal moves for the selected piece.
        /// Differentiates between normal moves and captures.
        /// </summary>
        public void HighlightLegalMoves(List<Move> legalMoves)
        {
            ClearMoveHighlights();

            foreach (Move move in legalMoves)
            {
                GameObject highlight = GetHighlightFromPool();
                Vector3 worldPos = boardVisualizer.BoardToWorldPosition(move.To);
                worldPos.z = -0.5f; // Between board and pieces

                highlight.transform.position = worldPos;

                // Use different color for captures
                bool isCapture = move.CapturedPiece != null || move.Type == MoveType.EnPassant;
                highlight.GetComponent<SpriteRenderer>().color = isCapture ? captureColor : legalMoveColor;

                highlight.SetActive(true);
                activeHighlights.Add(highlight);
            }
        }

        /// <summary>
        /// Clears the selected square highlight.
        /// </summary>
        public void ClearSelectedHighlight()
        {
            if (selectedHighlight != null)
            {
                ReturnHighlightToPool(selectedHighlight);
                selectedHighlight = null;
            }
        }

        /// <summary>
        /// Clears all legal move highlights.
        /// </summary>
        public void ClearMoveHighlights()
        {
            foreach (GameObject highlight in activeHighlights)
            {
                ReturnHighlightToPool(highlight);
            }
            activeHighlights.Clear();
        }

        /// <summary>
        /// Clears all highlights (selected and legal moves).
        /// </summary>
        public void ClearAllHighlights()
        {
            ClearSelectedHighlight();
            ClearMoveHighlights();
        }

        /// <summary>
        /// Gets a highlight GameObject from the pool, creating a new one if needed.
        /// </summary>
        private GameObject GetHighlightFromPool()
        {
            if (highlightPool.Count > 0)
            {
                return highlightPool.Dequeue();
            }
            else
            {
                // Pool exhausted, create a new highlight
                return CreateHighlight();
            }
        }

        /// <summary>
        /// Returns a highlight GameObject to the pool for reuse.
        /// </summary>
        private void ReturnHighlightToPool(GameObject highlight)
        {
            if (highlight != null)
            {
                highlight.SetActive(false);
                highlightPool.Enqueue(highlight);
            }
        }

        private void OnDestroy()
        {
            // Clean up all highlights
            ClearAllHighlights();

            while (highlightPool.Count > 0)
            {
                GameObject highlight = highlightPool.Dequeue();
                if (highlight != null)
                {
                    Destroy(highlight);
                }
            }
        }
    }
}
