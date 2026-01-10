using UnityEngine;
using ChessGame.Core.Board;
using ChessGame.Core.Pieces;

namespace ChessGame.Core.GameLogic
{
    /// <summary>
    /// Sets up the initial Capablanca chess position on the board.
    /// </summary>
    public class BoardSetup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ChessBoard chessBoard;
        [SerializeField] private BoardVisualizer boardVisualizer;

        [Header("Piece Sprites")]
        [SerializeField] private Sprite whitePawnSprite;
        [SerializeField] private Sprite whiteRookSprite;
        [SerializeField] private Sprite whiteKnightSprite;
        [SerializeField] private Sprite whiteBishopSprite;
        [SerializeField] private Sprite whiteQueenSprite;
        [SerializeField] private Sprite whiteKingSprite;
        [SerializeField] private Sprite whiteArchbishopSprite;
        [SerializeField] private Sprite whiteChancellorSprite;

        [SerializeField] private Sprite blackPawnSprite;
        [SerializeField] private Sprite blackRookSprite;
        [SerializeField] private Sprite blackKnightSprite;
        [SerializeField] private Sprite blackBishopSprite;
        [SerializeField] private Sprite blackQueenSprite;
        [SerializeField] private Sprite blackKingSprite;
        [SerializeField] private Sprite blackArchbishopSprite;
        [SerializeField] private Sprite blackChancellorSprite;

        /// <summary>
        /// Sets up the Capablanca starting position.
        /// White back rank: R-N-A-B-Q-K-B-C-N-R (files 0-9)
        /// Black back rank: R-N-A-B-Q-K-B-C-N-R (files 0-9)
        /// </summary>
        public void SetupCapablancaPosition()
        {
            // Clear any existing pieces
            chessBoard.ClearBoard();

            // Setup White pieces (rank 0 and 1)
            SetupBackRank(PieceColor.White, 0);
            SetupPawnRank(PieceColor.White, 1);

            // Setup Black pieces (rank 7 and 6)
            SetupBackRank(PieceColor.Black, 7);
            SetupPawnRank(PieceColor.Black, 6);
        }

        /// <summary>
        /// Sets up the back rank for a color with Capablanca piece arrangement.
        /// </summary>
        private void SetupBackRank(PieceColor color, int rank)
        {
            // Capablanca starting position: R-N-A-B-Q-K-B-C-N-R
            var pieceTypes = new[]
            {
                PieceType.Rook,         // File 0 (a)
                PieceType.Knight,       // File 1 (b)
                PieceType.Archbishop,   // File 2 (c)
                PieceType.Bishop,       // File 3 (d)
                PieceType.Queen,        // File 4 (e)
                PieceType.King,         // File 5 (f)
                PieceType.Bishop,       // File 6 (g)
                PieceType.Chancellor,   // File 7 (h)
                PieceType.Knight,       // File 8 (i)
                PieceType.Rook          // File 9 (j)
            };

            for (int file = 0; file < 10; file++)
            {
                BoardPosition position = new BoardPosition(file, rank);
                CreatePiece(pieceTypes[file], color, position);
            }
        }

        /// <summary>
        /// Sets up a rank of pawns.
        /// </summary>
        private void SetupPawnRank(PieceColor color, int rank)
        {
            for (int file = 0; file < 10; file++)
            {
                BoardPosition position = new BoardPosition(file, rank);
                CreatePiece(PieceType.Pawn, color, position);
            }
        }

        /// <summary>
        /// Creates a piece GameObject with the appropriate component and sprite.
        /// </summary>
        private ChessPiece CreatePiece(PieceType type, PieceColor color, BoardPosition position)
        {
            // Create GameObject
            GameObject pieceObj = new GameObject($"{color}_{type}_{position.ToAlgebraic()}");
            pieceObj.transform.parent = chessBoard.transform;

            // Position in world space
            Vector3 worldPos = boardVisualizer.GetPieceWorldPosition(position);
            pieceObj.transform.position = worldPos;

            // Add SpriteRenderer
            SpriteRenderer renderer = pieceObj.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 7; // Pieces above board
            renderer.sprite = GetSpriteForPiece(type, color);

            // Add appropriate piece component
            ChessPiece piece = null;
            switch (type)
            {
                case PieceType.Pawn:
                    piece = pieceObj.AddComponent<Pawn>();
                    break;
                case PieceType.Rook:
                    piece = pieceObj.AddComponent<Rook>();
                    break;
                case PieceType.Knight:
                    piece = pieceObj.AddComponent<Knight>();
                    break;
                case PieceType.Bishop:
                    piece = pieceObj.AddComponent<Bishop>();
                    break;
                case PieceType.Queen:
                    piece = pieceObj.AddComponent<Queen>();
                    break;
                case PieceType.King:
                    piece = pieceObj.AddComponent<King>();
                    break;
                case PieceType.Archbishop:
                    piece = pieceObj.AddComponent<Archbishop>();
                    break;
                case PieceType.Chancellor:
                    piece = pieceObj.AddComponent<Chancellor>();
                    break;
            }

            if (piece != null)
            {
                piece.Initialize(type, color, position);
                chessBoard.SetPiece(position, piece);
            }

            return piece;
        }

        /// <summary>
        /// Gets the sprite for a specific piece type and color.
        /// </summary>
        private Sprite GetSpriteForPiece(PieceType type, PieceColor color)
        {
            if (color == PieceColor.White)
            {
                return type switch
                {
                    PieceType.Pawn => whitePawnSprite,
                    PieceType.Rook => whiteRookSprite,
                    PieceType.Knight => whiteKnightSprite,
                    PieceType.Bishop => whiteBishopSprite,
                    PieceType.Queen => whiteQueenSprite,
                    PieceType.King => whiteKingSprite,
                    PieceType.Archbishop => whiteArchbishopSprite,
                    PieceType.Chancellor => whiteChancellorSprite,
                    _ => null
                };
            }
            else
            {
                return type switch
                {
                    PieceType.Pawn => blackPawnSprite,
                    PieceType.Rook => blackRookSprite,
                    PieceType.Knight => blackKnightSprite,
                    PieceType.Bishop => blackBishopSprite,
                    PieceType.Queen => blackQueenSprite,
                    PieceType.King => blackKingSprite,
                    PieceType.Archbishop => blackArchbishopSprite,
                    PieceType.Chancellor => blackChancellorSprite,
                    _ => null
                };
            }
        }
    }
}
