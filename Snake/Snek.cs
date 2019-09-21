﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Snake.ArenaItems;

namespace Snake
{
    public enum Direction
    {
        Up, Down, Left, Right
    }

    public class Snek
    {
        /// <summary>
        /// Last element is the front of the snake. First element is the tail.
        /// </summary>
        public LinkedList<Point> Body;

        private readonly Arena _arena;
        private Point _speed;

        public int Growth { get; set; } = 2;

        public Direction Direction { get; set; }
        public int SpeedModifier { get; set; }

        public Brush Color { get; internal set; }

        public Snek(Arena arena, Point spawnPoint)
            : this(arena, spawnPoint, new LinkedList<Point>(), Direction.Right, 1, Brushes.Firebrick)
        {
        }

        public Snek(Arena arena, Point spawnPoint, LinkedList<Point> body, Direction direction, int speedModifier, Brush color)
        {
            _arena = arena;

            Body = body;
            SpeedModifier = speedModifier;

            Body.AddLast(spawnPoint);
            ChangeDirection(direction);

            Color = color;
        }

        public void Move()
        {
            var newHead = GetNewHead();

            // If an object on the ground exists, interact with it.
            _arena.Cells[newHead.X, newHead.Y]?.Interact(this);

            // Set the currently visited cell as the player's.
            _arena.Cells[newHead.X, newHead.Y] = new SnekBody(Color, Body.AddLast(newHead));

            // Free the tail cell if snake is not growing.
            var tail = Body.First.Value;
            if (Growth > 0)
                Growth--;
            else
            {
                _arena.Cells[tail.X, tail.Y] = null;
                Body.RemoveFirst();
            }
        }

        public void ChangeDirection(Direction newDirection)
        {
            switch (newDirection)
            {
                case Direction.Up:
                    if (Direction != Direction.Down)
                    {
                        _speed.X = 0;
                        _speed.Y = -1;
                        Direction = newDirection;
                    }
                    break;

                case Direction.Down:
                    if (Direction != Direction.Up)
                    {
                        _speed.X = 0;
                        _speed.Y = 1;
                        Direction = newDirection;
                    }
                    break;

                case Direction.Left:
                    if (Direction != Direction.Right)
                    {
                        _speed.X = -1;
                        _speed.Y = 0;
                        Direction = newDirection;
                    }
                    break;

                case Direction.Right:
                    if (Direction != Direction.Left)
                    {
                        _speed.X = 1;
                        _speed.Y = 0;
                        Direction = newDirection;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newDirection), newDirection, null);
            }

            _speed.X *= SpeedModifier;
            _speed.Y *= SpeedModifier;
        }

        private Point GetNewHead()
        {
            var head = Body.Last.Value;

            var newX = (head.X + _speed.X) % _arena.Width;
            if (newX < 0)
            {
                newX += _arena.Width;
            }

            var newY = (head.Y + _speed.Y) % _arena.Height;
            if (newY < 0)
            {
                newY += _arena.Height;
            }

            var newHead = new Point(newX, newY);
            return newHead;
        }

        public void TrimTail(LinkedListNode<Point> cutoffPoint)
        {
            // Trim the list until snake was eaten
            while (Body.First != cutoffPoint)
            {
                var cell = Body.First.Value;
                Body.RemoveFirst();
                _arena.Cells[cell.X, cell.Y] = null;
            }
            
            // Trim it once more, because it was trimmed up until the trim point. Exempt if it would obliterate the snake.
            if (Body.Count > 1)
            {
                var cell = Body.First.Value;
                Body.RemoveFirst();
                _arena.Cells[cell.X, cell.Y] = null;
            }
        }
    }
}