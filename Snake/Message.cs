using System.Collections.Generic;
using System.Drawing;

namespace Snake
{
    public enum Direction
    {
        Up, Down, Left, Right
    }

    public enum GameStateEnum
    {
        Pregame = 0,        // The game hasn't started yet, but it's about to start soon
        InProgress = 1,     // Game is currently in progress
        EndingSoon = 2,     // Game is in progress, but is about to end soon
        PostGame = 3,       // Game has ended
        Unknown = 4         // Initial value for clients joining the game
                            // (refer to SnakeMainForm.JoinArena for how it is used)
    }

    public class Message
    {
        public Dictionary<Point, Color> Arena { get; set; }
        // State of the game (pregame, in progress, about to end soon, post-game)
        public GameStateEnum GameState { get; set; }
        // Player standings data for the top three players (only sent post-game, otherwise it is null)
        public PlayerStandingsData[] Podium;
    }

    public class PlayerStandingsData
    {
        public Color PlayerColor { get; }
        public int SnakeLength { get; }

        public PlayerStandingsData(Color playerColor, int snakeLength)
        {
            PlayerColor = playerColor;
            SnakeLength = snakeLength;
        }
    }
}