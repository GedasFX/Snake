﻿using System.Collections.Generic;
using System.Drawing;

namespace Server.ArenaItems
{
    internal class SnekBody : ISnekBody
    {
        private readonly LinkedListNode<Point> _bodyNode;

        public Color Color { get; }

        public SnekBody(Color color, LinkedListNode<Point> bodyNode)
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