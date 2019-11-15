using System.Drawing;

namespace Server
{
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