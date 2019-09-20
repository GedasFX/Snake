using System.Collections.Generic;
using System.Drawing;

namespace Snake.ArenaItems
{
    internal class SnekBody : ISnekBody
    {
        private readonly LinkedListNode<Point> _bodyNode;

        public SnekBody(LinkedListNode<Point> bodyNode)
        {
            _bodyNode = bodyNode;
        }

        public void Interact(Snek snek)
        {
            var list = _bodyNode.List;
            if (list == null)
                return;

            // Trim the list until snake was eaten
            while (list.First != _bodyNode)
                list.RemoveFirst();

            // Trim it once more, because it was trimmed up until the trim point. Exempt if low list.
            if (list.Count > 1)
                list.RemoveFirst();
        }
    }
}