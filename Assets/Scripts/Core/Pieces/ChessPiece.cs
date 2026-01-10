using System.Collections.Generic;
using UnityEngine;
using ChessGame.Core.Board;
using ChessGame.Core.MoveSystem;

namespace ChessGame.Core.Pieces
{
    /// <summary>
    /// Abstract base class for all chess pieces.
    /// Each piece type implements its own movement rules via GetPseudoLegalMoves().
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class ChessPiece : MonoBehaviour
    {
        [Header("Piece Properties")]
        [SerializeField] protected PieceType type;
        [SerializeField] protected PieceColor color;

        protected SpriteRenderer spriteRenderer;

        /// <summary>
        /// The type of this piece (Pawn, Rook, Knight, etc.).
        /// </summary>
        public PieceType Type => type;

        /// <summary>
        /// The color of this piece (White or Black).
        /// </summary>
        public PieceColor Color => color;

        /// <summary>
        /// Current position on the board.
        /// </summary>
        public BoardPosition Position { get; set; }

        /// <summary>
        /// Whether this piece has moved from its starting position.
        /// Important for castling, pawn double-moves, and en passant.
        /// </summary>
        public bool HasMoved { get; set; }

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            HasMoved = false;
        }

        /// <summary>
        /// Initialize the piece with its type, color, and position.
        /// </summary>
        public virtual void Initialize(PieceType pieceType, PieceColor pieceColor, BoardPosition position)
        {
            type = pieceType;
            color = pieceColor;
            Position = position;
            HasMoved = false;

            // Set sorting order so pieces appear above the board
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 7;
            }
        }

        /// <summary>
        /// Gets all pseudo-legal moves for this piece.
        /// Pseudo-legal means moves following the piece's movement rules,
        /// but NOT accounting for check (i.e., may leave own king in check).
        /// Actual legal moves are filtered later by checking if they leave the king in check.
        /// </summary>
        public abstract List<Move> GetPseudoLegalMoves(ChessBoard board);

        /// <summary>
        /// Moves this piece to a new position.
        /// Updates the position and HasMoved flag.
        /// </summary>
        public virtual void MoveTo(BoardPosition newPosition)
        {
            Position = newPosition;
            HasMoved = true;
        }

        /// <summary>
        /// Called when this piece is captured.
        /// Override to add capture effects (animations, sounds, etc.).
        /// </summary>
        public virtual void OnCaptured()
        {
            // Override in derived classes for specific behavior
        }

        /// <summary>
        /// Sets the sprite for this piece.
        /// </summary>
        public void SetSprite(Sprite sprite)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
            }
        }

        /// <summary>
        /// Helper method: Gets sliding moves in specified directions (for Rook, Bishop, Queen, Archbishop, Chancellor).
        /// </summary>
        protected List<Move> GetSlidingMoves(ChessBoard board, (int dFile, int dRank)[] directions)
        {
            List<Move> moves = new List<Move>();

            foreach (var (dFile, dRank) in directions)
            {
                BoardPosition current = Position;

                // Slide in this direction until hitting edge or another piece
                while (true)
                {
                    current = current + (dFile, dRank);

                    if (!current.IsValid())
                        break; // Off the board

                    ChessPiece pieceAtTarget = board.GetPiece(current);

                    if (pieceAtTarget == null)
                    {
                        // Empty square - can move here
                        moves.Add(new Move(Position, current, this));
                    }
                    else
                    {
                        // Square occupied
                        if (pieceAtTarget.Color != this.Color)
                        {
                            // Enemy piece - can capture
                            moves.Add(new Move(Position, current, this, pieceAtTarget, MoveType.Capture));
                        }
                        // Can't move further in this direction (blocked by any piece)
                        break;
                    }
                }
            }

            return moves;
        }

        /// <summary>
        /// Helper method: Gets jumping moves (for Knight).
        /// </summary>
        protected List<Move> GetJumpingMoves(ChessBoard board, (int dFile, int dRank)[] offsets)
        {
            List<Move> moves = new List<Move>();

            foreach (var (dFile, dRank) in offsets)
            {
                BoardPosition targetPos = Position + (dFile, dRank);

                if (!targetPos.IsValid())
                    continue;

                ChessPiece pieceAtTarget = board.GetPiece(targetPos);

                if (pieceAtTarget == null)
                {
                    // Empty square
                    moves.Add(new Move(Position, targetPos, this));
                }
                else if (pieceAtTarget.Color != this.Color)
                {
                    // Enemy piece
                    moves.Add(new Move(Position, targetPos, this, pieceAtTarget, MoveType.Capture));
                }
                // Friendly piece - can't move there
            }

            return moves;
        }

        /// <summary>
        /// Gets the opposite color.
        /// </summary>
        protected PieceColor GetOppositeColor()
        {
            return Color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }

        public override string ToString()
        {
            return $"{Color} {Type} at {Position.ToAlgebraic()}";
        }
    }
}
