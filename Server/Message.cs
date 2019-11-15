using System.Collections.Generic;
using System.Drawing;
using Server.GameStates;

namespace Server
{
    public class Message
    {
        public Dictionary<Point, Color> Arena { get; }

        // State of the game (pregame, in progress, ending soon, post-game)
        public GameStateEnum GameState { get; }

        // Player standings data for the top three players (only sent post-game)
        public PlayerStandingsData[] Podium;

        public Message(Dictionary<Point, Color> colorMap, GameStateEnum state, PlayerStandingsData[] podium)
        {
            Arena = colorMap;
            GameState = state;
            Podium = podium;
        }
    }
}