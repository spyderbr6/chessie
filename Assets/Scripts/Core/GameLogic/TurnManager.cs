using System;
using UnityEngine;
using ChessGame.Core.Pieces;

namespace ChessGame.Core.GameLogic
{
    /// <summary>
    /// Manages turn order and player switching in the chess game.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        /// <summary>
        /// Event fired when the turn changes to a new player.
        /// </summary>
        public event Action<PieceColor> OnTurnChanged;

        /// <summary>
        /// The color of the player whose turn it currently is.
        /// </summary>
        public PieceColor CurrentTurn { get; private set; }

        /// <summary>
        /// The current turn number (increments after Black moves).
        /// </summary>
        public int TurnNumber { get; private set; }

        /// <summary>
        /// Half-move clock for the 50-move rule.
        /// Increments on each move, resets on pawn moves or captures.
        /// </summary>
        public int HalfMoveClock { get; private set; }

        /// <summary>
        /// Starts a new game with White to move.
        /// </summary>
        public void StartNewGame()
        {
            CurrentTurn = PieceColor.White;
            TurnNumber = 1;
            HalfMoveClock = 0;

            OnTurnChanged?.Invoke(CurrentTurn);
        }

        /// <summary>
        /// Ends the current player's turn and switches to the opponent.
        /// </summary>
        public void EndTurn()
        {
            // Switch current player
            CurrentTurn = GetOppositeColor(CurrentTurn);

            // Increment turn number after Black moves
            if (CurrentTurn == PieceColor.White)
            {
                TurnNumber++;
            }

            OnTurnChanged?.Invoke(CurrentTurn);
        }

        /// <summary>
        /// Checks if the specified color is allowed to move (is it their turn?).
        /// </summary>
        public bool CanPlayerMove(PieceColor color)
        {
            return color == CurrentTurn;
        }

        /// <summary>
        /// Increments the half-move clock (for 50-move rule tracking).
        /// </summary>
        public void IncrementHalfMoveClock()
        {
            HalfMoveClock++;
        }

        /// <summary>
        /// Resets the half-move clock (called after pawn moves or captures).
        /// </summary>
        public void ResetHalfMoveClock()
        {
            HalfMoveClock = 0;
        }

        /// <summary>
        /// Checks if the 50-move rule draw condition is met.
        /// </summary>
        public bool IsFiftyMoveRule()
        {
            return HalfMoveClock >= 100; // 100 half-moves = 50 full moves
        }

        /// <summary>
        /// Gets the opposite color.
        /// </summary>
        public PieceColor GetOppositeColor(PieceColor color)
        {
            return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }

        /// <summary>
        /// Resets the turn manager to initial state.
        /// </summary>
        public void Reset()
        {
            CurrentTurn = PieceColor.White;
            TurnNumber = 1;
            HalfMoveClock = 0;
        }

        public override string ToString()
        {
            return $"Turn {TurnNumber}, {CurrentTurn} to move (Half-move clock: {HalfMoveClock})";
        }
    }
}
