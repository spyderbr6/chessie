namespace ChessGame.Core.GameLogic
{
    /// <summary>
    /// Represents the current state of the chess game.
    /// </summary>
    public enum GameState
    {
        NotStarted,        // Game has not been initialized
        InProgress,        // Game is actively being played
        AwaitingPromotion, // Waiting for player to choose promotion piece
        Checkmate,         // Game ended in checkmate
        Stalemate,         // Game ended in stalemate
        Draw               // Game ended in a draw (50-move rule, threefold repetition, etc.)
    }
}
