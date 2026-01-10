using System;

namespace ChessGame.Core.Board
{
    /// <summary>
    /// Immutable struct representing a position on the 10x8 Capablanca chess board.
    /// Files are 0-9 (columns a-j), Ranks are 0-7 (rows 1-8).
    /// </summary>
    public readonly struct BoardPosition : IEquatable<BoardPosition>
    {
        public readonly int File; // 0-9 (a-j)
        public readonly int Rank; // 0-7 (1-8)

        public BoardPosition(int file, int rank)
        {
            File = file;
            Rank = rank;
        }

        /// <summary>
        /// Checks if this position is within the valid board bounds (10x8).
        /// </summary>
        public bool IsValid()
        {
            return File >= 0 && File < 10 && Rank >= 0 && Rank < 8;
        }

        /// <summary>
        /// Converts the position to algebraic notation (e.g., "a1", "j8").
        /// </summary>
        public string ToAlgebraic()
        {
            if (!IsValid())
                return "invalid";

            char fileChar = (char)('a' + File);
            int rankNum = Rank + 1;
            return $"{fileChar}{rankNum}";
        }

        /// <summary>
        /// Parses algebraic notation (e.g., "a1", "j8") into a BoardPosition.
        /// </summary>
        public static BoardPosition FromAlgebraic(string notation)
        {
            if (string.IsNullOrEmpty(notation) || notation.Length < 2)
                throw new ArgumentException($"Invalid algebraic notation: {notation}");

            char fileChar = char.ToLower(notation[0]);
            int file = fileChar - 'a';

            if (!int.TryParse(notation.Substring(1), out int rank))
                throw new ArgumentException($"Invalid rank in notation: {notation}");

            return new BoardPosition(file, rank - 1);
        }

        /// <summary>
        /// Adds a delta (file, rank) offset to this position.
        /// </summary>
        public static BoardPosition operator +(BoardPosition pos, (int dFile, int dRank) delta)
        {
            return new BoardPosition(pos.File + delta.dFile, pos.Rank + delta.dRank);
        }

        /// <summary>
        /// Subtracts a delta (file, rank) offset from this position.
        /// </summary>
        public static BoardPosition operator -(BoardPosition pos, (int dFile, int dRank) delta)
        {
            return new BoardPosition(pos.File - delta.dFile, pos.Rank - delta.dRank);
        }

        // Equality implementation
        public bool Equals(BoardPosition other)
        {
            return File == other.File && Rank == other.Rank;
        }

        public override bool Equals(object obj)
        {
            return obj is BoardPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(File, Rank);
        }

        public static bool operator ==(BoardPosition left, BoardPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BoardPosition left, BoardPosition right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return ToAlgebraic();
        }
    }
}
