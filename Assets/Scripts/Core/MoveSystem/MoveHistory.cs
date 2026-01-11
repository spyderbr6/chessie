using System.Collections.Generic;
using UnityEngine;

namespace ChessGame.Core.MoveSystem
{
    /// <summary>
    /// Tracks the history of moves played in the game.
    /// Used for move notation, undo/redo, and game replay.
    /// </summary>
    public class MoveHistory : MonoBehaviour
    {
        private List<Move> moves = new List<Move>();

        /// <summary>
        /// Gets the total number of moves played.
        /// </summary>
        public int MoveCount => moves.Count;

        /// <summary>
        /// Gets a read-only list of all moves.
        /// </summary>
        public IReadOnlyList<Move> Moves => moves.AsReadOnly();

        /// <summary>
        /// Adds a move to the history.
        /// </summary>
        public void AddMove(Move move)
        {
            moves.Add(move);
            Debug.Log($"Move {moves.Count}: {move}");
        }

        /// <summary>
        /// Gets the most recent move, or null if no moves have been played.
        /// </summary>
        public Move? GetLastMove()
        {
            if (moves.Count == 0)
                return null;

            return moves[moves.Count - 1];
        }

        /// <summary>
        /// Gets the move at the specified index (0-based).
        /// </summary>
        public Move? GetMove(int index)
        {
            if (index < 0 || index >= moves.Count)
                return null;

            return moves[index];
        }

        /// <summary>
        /// Clears the move history (used when starting a new game).
        /// </summary>
        public void Clear()
        {
            moves.Clear();
            Debug.Log("Move history cleared");
        }

        /// <summary>
        /// Gets the algebraic notation for all moves (simplified).
        /// Format: "e2-e4 e7-e5 Ng1-f3..."
        /// </summary>
        public string GetAlgebraicNotation()
        {
            if (moves.Count == 0)
                return "(no moves)";

            List<string> notation = new List<string>();

            foreach (Move move in moves)
            {
                notation.Add($"{move.From.ToAlgebraic()}-{move.To.ToAlgebraic()}");
            }

            return string.Join(" ", notation);
        }

        /// <summary>
        /// Gets move history formatted as a numbered list.
        /// Format: "1. e2-e4 e7-e5  2. Ng1-f3 Nb8-c6..."
        /// </summary>
        public string GetFormattedMoveList()
        {
            if (moves.Count == 0)
                return "(no moves)";

            List<string> lines = new List<string>();

            for (int i = 0; i < moves.Count; i += 2)
            {
                int moveNumber = (i / 2) + 1;
                string whiteMoveNotation = $"{moves[i].From.ToAlgebraic()}-{moves[i].To.ToAlgebraic()}";

                if (i + 1 < moves.Count)
                {
                    string blackMoveNotation = $"{moves[i + 1].From.ToAlgebraic()}-{moves[i + 1].To.ToAlgebraic()}";
                    lines.Add($"{moveNumber}. {whiteMoveNotation} {blackMoveNotation}");
                }
                else
                {
                    lines.Add($"{moveNumber}. {whiteMoveNotation}");
                }
            }

            return string.Join("  ", lines);
        }

        /// <summary>
        /// Prints the move history to the console.
        /// </summary>
        public void PrintMoveHistory()
        {
            Debug.Log($"Move History ({moves.Count} moves):");
            Debug.Log(GetFormattedMoveList());
        }
    }
}
