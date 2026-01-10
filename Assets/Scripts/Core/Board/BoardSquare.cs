using UnityEngine;

namespace ChessGame.Core.Board
{
    /// <summary>
    /// Component attached to each board square to store its position.
    /// Used for raycasting and converting mouse clicks to board positions.
    /// </summary>
    public class BoardSquare : MonoBehaviour
    {
        public BoardPosition Position { get; set; }

        public override string ToString()
        {
            return $"BoardSquare at {Position.ToAlgebraic()}";
        }
    }
}
