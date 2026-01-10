using System;
using ChessGame.Core.Board;
using ChessGame.Core.Pieces;

namespace ChessGame.Core.MoveSystem
{
    /// <summary>
    /// Immutable struct representing a chess move with all associated data.
    /// </summary>
    public readonly struct Move : IEquatable<Move>
    {
        public readonly BoardPosition From;
        public readonly BoardPosition To;
        public readonly MoveType Type;
        public readonly ChessPiece MovingPiece;
        public readonly ChessPiece CapturedPiece; // null if no capture
        public readonly PieceType PromotionType;  // None if not a promotion

        // Special move data
        public readonly bool IsCastling;
        public readonly bool IsEnPassant;
        public readonly bool IsPromotion;
        public readonly BoardPosition? RookMoveFrom; // For castling
        public readonly BoardPosition? RookMoveTo;   // For castling

        /// <summary>
        /// Standard move constructor.
        /// </summary>
        public Move(
            BoardPosition from,
            BoardPosition to,
            ChessPiece movingPiece,
            ChessPiece capturedPiece = null,
            MoveType type = MoveType.Normal)
        {
            From = from;
            To = to;
            MovingPiece = movingPiece;
            CapturedPiece = capturedPiece;
            Type = type;
            PromotionType = PieceType.None;

            IsCastling = type == MoveType.Castling;
            IsEnPassant = type == MoveType.EnPassant;
            IsPromotion = type == MoveType.Promotion;
            RookMoveFrom = null;
            RookMoveTo = null;
        }

        /// <summary>
        /// Private constructor with all fields (used by factory methods).
        /// </summary>
        private Move(
            BoardPosition from,
            BoardPosition to,
            ChessPiece movingPiece,
            ChessPiece capturedPiece,
            MoveType type,
            PieceType promotionType,
            bool isCastling,
            bool isEnPassant,
            bool isPromotion,
            BoardPosition? rookMoveFrom,
            BoardPosition? rookMoveTo)
        {
            From = from;
            To = to;
            MovingPiece = movingPiece;
            CapturedPiece = capturedPiece;
            Type = type;
            PromotionType = promotionType;
            IsCastling = isCastling;
            IsEnPassant = isEnPassant;
            IsPromotion = isPromotion;
            RookMoveFrom = rookMoveFrom;
            RookMoveTo = rookMoveTo;
        }

        /// <summary>
        /// Creates a castling move.
        /// </summary>
        public static Move CreateCastling(
            BoardPosition kingFrom,
            BoardPosition kingTo,
            ChessPiece king,
            BoardPosition rookFrom,
            BoardPosition rookTo)
        {
            return new Move(
                from: kingFrom,
                to: kingTo,
                movingPiece: king,
                capturedPiece: null,
                type: MoveType.Castling,
                promotionType: PieceType.None,
                isCastling: true,
                isEnPassant: false,
                isPromotion: false,
                rookMoveFrom: rookFrom,
                rookMoveTo: rookTo
            );
        }

        /// <summary>
        /// Creates an en passant capture move.
        /// </summary>
        public static Move CreateEnPassant(
            BoardPosition from,
            BoardPosition to,
            ChessPiece movingPawn,
            ChessPiece capturedPawn)
        {
            return new Move(
                from: from,
                to: to,
                movingPiece: movingPawn,
                capturedPiece: capturedPawn,
                type: MoveType.EnPassant,
                promotionType: PieceType.None,
                isCastling: false,
                isEnPassant: true,
                isPromotion: false,
                rookMoveFrom: null,
                rookMoveTo: null
            );
        }

        /// <summary>
        /// Creates a pawn promotion move.
        /// </summary>
        public static Move CreatePromotion(
            BoardPosition from,
            BoardPosition to,
            ChessPiece movingPawn,
            PieceType promotionType,
            ChessPiece capturedPiece = null)
        {
            MoveType type = capturedPiece != null ? MoveType.Capture : MoveType.Promotion;

            return new Move(
                from: from,
                to: to,
                movingPiece: movingPawn,
                capturedPiece: capturedPiece,
                type: type,
                promotionType: promotionType,
                isCastling: false,
                isEnPassant: false,
                isPromotion: true,
                rookMoveFrom: null,
                rookMoveTo: null
            );
        }

        // Equality implementation (based on from/to positions)
        public bool Equals(Move other)
        {
            return From.Equals(other.From) &&
                   To.Equals(other.To) &&
                   Type == other.Type &&
                   PromotionType == other.PromotionType;
        }

        public override bool Equals(object obj)
        {
            return obj is Move other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To, Type, PromotionType);
        }

        public static bool operator ==(Move left, Move right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Move left, Move right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            string result = $"{From.ToAlgebraic()}-{To.ToAlgebraic()}";

            if (IsCastling)
                result += " (Castling)";
            else if (IsEnPassant)
                result += " (En Passant)";
            else if (IsPromotion)
                result += $" (Promotion to {PromotionType})";
            else if (CapturedPiece != null)
                result += $" (Captures {CapturedPiece.Type})";

            return result;
        }
    }
}
