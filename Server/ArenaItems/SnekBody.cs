using System.Collections.Generic;
using System.Drawing;

namespace Server.ArenaItems
{
    internal class SnekBody : ISnekBody
    {
        private readonly LinkedListNode<Point> _bodyNode;

        private Snek prevSnek;

        public Color Color { get; }

        public SnekBody(Color color, LinkedListNode<Point> bodyNode)
        {
            _bodyNode = bodyNode;

            Color = color;
        }

        public void Interact(Snek snek)
        {
            if (snek.Body.Last == _bodyNode)
                snek.ChangeDirection(Direction.None);
            else
                snek.TrimTail(_bodyNode);
            
            // HUGE unecessary memory leak. For demonstration purposes only.
            //prevSnek = snek.Clone() as Snek;
        }

        public void Undo(Snek snek)
        {
            if (prevSnek != null)
                snek.Body = prevSnek.Body;
        }
    }
}