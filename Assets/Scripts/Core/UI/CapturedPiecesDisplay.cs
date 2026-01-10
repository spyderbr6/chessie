using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ChessGame.Core.Pieces;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.UI
{
    /// <summary>
    /// Displays captured pieces for both players.
    /// Shows piece counts and material advantage.
    /// </summary>
    public class CapturedPiecesDisplay : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text whiteCapturedText;
        [SerializeField] private TMP_Text blackCapturedText;
        [SerializeField] private TMP_Text materialAdvantageText;

        [Header("Game References")]
        [SerializeField] private MoveExecutor moveExecutor;

        private List<ChessPiece> whiteCapturedPieces = new List<ChessPiece>();
        private List<ChessPiece> blackCapturedPieces = new List<ChessPiece>();

        // Piece values for material calculation
        private readonly Dictionary<PieceType, int> pieceValues = new Dictionary<PieceType, int>
        {
            { PieceType.Pawn, 1 },
            { PieceType.Knight, 3 },
            { PieceType.Bishop, 3 },
            { PieceType.Rook, 5 },
            { PieceType.Archbishop, 6 }, // Bishop + Knight
            { PieceType.Chancellor, 8 },  // Rook + Knight
            { PieceType.Queen, 9 },
            { PieceType.King, 0 } // King has no material value (can't be captured in normal play)
        };

        private void OnEnable()
        {
            if (moveExecutor != null)
            {
                moveExecutor.OnPieceCaptured += HandlePieceCaptured;
            }
        }

        private void OnDisable()
        {
            if (moveExecutor != null)
            {
                moveExecutor.OnPieceCaptured -= HandlePieceCaptured;
            }
        }

        private void Start()
        {
            UpdateDisplay();
        }

        /// <summary>
        /// Called when a piece is captured.
        /// </summary>
        private void HandlePieceCaptured(ChessPiece capturedPiece)
        {
            if (capturedPiece == null)
                return;

            // Add to appropriate list
            if (capturedPiece.Color == PieceColor.White)
            {
                whiteCapturedPieces.Add(capturedPiece);
            }
            else
            {
                blackCapturedPieces.Add(capturedPiece);
            }

            UpdateDisplay();
        }

        /// <summary>
        /// Updates all captured piece displays.
        /// </summary>
        private void UpdateDisplay()
        {
            UpdateCapturedPiecesText();
            UpdateMaterialAdvantage();
        }

        /// <summary>
        /// Updates the captured pieces text for both colors.
        /// </summary>
        private void UpdateCapturedPiecesText()
        {
            if (whiteCapturedText != null)
            {
                whiteCapturedText.text = $"White Captured:\n{GetCapturedPiecesString(whiteCapturedPieces)}";
            }

            if (blackCapturedText != null)
            {
                blackCapturedText.text = $"Black Captured:\n{GetCapturedPiecesString(blackCapturedPieces)}";
            }
        }

        /// <summary>
        /// Gets a formatted string of captured pieces.
        /// </summary>
        private string GetCapturedPiecesString(List<ChessPiece> pieces)
        {
            if (pieces.Count == 0)
                return "None";

            // Count pieces by type
            Dictionary<PieceType, int> pieceCounts = new Dictionary<PieceType, int>();

            foreach (ChessPiece piece in pieces)
            {
                if (pieceCounts.ContainsKey(piece.Type))
                {
                    pieceCounts[piece.Type]++;
                }
                else
                {
                    pieceCounts[piece.Type] = 1;
                }
            }

            // Build string
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var kvp in pieceCounts)
            {
                string pieceName = GetPieceSymbol(kvp.Key);
                sb.Append($"{pieceName}×{kvp.Value} ");
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Gets a symbol/abbreviation for a piece type.
        /// </summary>
        private string GetPieceSymbol(PieceType type)
        {
            return type switch
            {
                PieceType.Pawn => "P",
                PieceType.Knight => "N",
                PieceType.Bishop => "B",
                PieceType.Rook => "R",
                PieceType.Archbishop => "A",
                PieceType.Chancellor => "C",
                PieceType.Queen => "Q",
                PieceType.King => "K",
                _ => "?"
            };
        }

        /// <summary>
        /// Calculates and displays material advantage.
        /// </summary>
        private void UpdateMaterialAdvantage()
        {
            if (materialAdvantageText == null)
                return;

            int whiteMaterial = CalculateMaterialValue(whiteCapturedPieces);
            int blackMaterial = CalculateMaterialValue(blackCapturedPieces);

            int advantage = blackMaterial - whiteMaterial; // Positive means White ahead

            if (advantage == 0)
            {
                materialAdvantageText.text = "Material: Equal";
            }
            else if (advantage > 0)
            {
                materialAdvantageText.text = $"Material: White +{advantage}";
            }
            else
            {
                materialAdvantageText.text = $"Material: Black +{-advantage}";
            }
        }

        /// <summary>
        /// Calculates total material value of captured pieces.
        /// </summary>
        private int CalculateMaterialValue(List<ChessPiece> pieces)
        {
            int total = 0;

            foreach (ChessPiece piece in pieces)
            {
                if (pieceValues.TryGetValue(piece.Type, out int value))
                {
                    total += value;
                }
            }

            return total;
        }

        /// <summary>
        /// Clears all captured piece data.
        /// </summary>
        public void Clear()
        {
            whiteCapturedPieces.Clear();
            blackCapturedPieces.Clear();
            UpdateDisplay();
        }
    }
}
