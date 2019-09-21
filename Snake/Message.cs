using System.Collections.Generic;
using System.Drawing;

namespace Snake
{
    public enum Direction
    {
        Up, Down, Left, Right
    }

    public class Message
    {
        public Dictionary<Point, Color> Arena { get; set; }
    }
}