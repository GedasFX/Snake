using System.Collections.Generic;
using System.Drawing;

namespace Server
{
    public class Message
    {
        public Dictionary<Point, Color> Arena { get; }

        public Message(Dictionary<Point, Color> arena)
        {
            Arena = arena;
        }
    }
}