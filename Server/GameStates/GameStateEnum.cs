namespace Server.GameStates
{
    /// <summary>
    /// Used while sending messages to clients about the current state of the game.
    /// </summary>
    public enum GameStateEnum
    {
        Pregame = 0,        // The game hasn't started yet, but it's about to start soon
        InProgress = 1,     // Game is currently in progress
        EndingSoon = 2,     // Game is in progress, but is about to end soon
        PostGame = 3,       // Game has ended
        Unknown = 4,        // Initial value for clients joining the game
                            // (refer to SnakeMainForm.JoinArena for how it is used)
        Pending = 5         // For completion purposes, not used
    }
}