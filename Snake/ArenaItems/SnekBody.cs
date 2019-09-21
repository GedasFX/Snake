using System.Collections.Generic;
using System.Drawing;

namespace Snake.ArenaItems
{
    internal class SnekBody : ISnekBody
    {
        private readonly LinkedListNode<Point> _bodyNode;

        public Brush Color { get; }

        public SnekBody(Brush color, LinkedListNode<Point> bodyNode)
        {
            _bodyNode = bodyNode;

            Color = color;
        }

        public void Interact(Snek snek)
        {
            snek.TrimTail(_bodyNode);
        }
    }
}