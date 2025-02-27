﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Server.ArenaItems;

namespace Server
{
    public enum Direction
    {
        Up, Down, Left, Right, None
    }

    public class Snek : ICloneable
    {
        /// <summary>
        /// Last element is the front of the snake. First element is the tail.
        /// </summary>
        public LinkedList<Point> Body { get; set; }

        public Arena Arena { get; }
        private Point _speed;

        public int Growth { get; set; } = 2;

        // In edge cases you can quickly change direction twice to do a 180 in a single frame and instantly eat yourself.
        // This adds a 1 frame delay buffer, so you cannot turn 180 degrees in a single frame.
        public Direction CurrentDirection { get; set; }
        public Direction NextDirection { get; set; }

        public int SpeedModifier { get; set; }

        public Color Color { get; }

        public Snek(Arena arena, Point spawnPoint)
            : this(arena, spawnPoint, new LinkedList<Point>(), Direction.Right, 1, Color.Firebrick)
        {
        }

        public Snek(Arena arena, Point spawnPoint, Color color)
            : this(arena, spawnPoint, new LinkedList<Point>(), Direction.Right, 1, color)
        {
        }

        public Snek(Arena arena, Point spawnPoint, LinkedList<Point> body, Direction direction, int speedModifier, Color color)
        {
            Arena = arena;

            Body = body;
            SpeedModifier = speedModifier;

            Body.AddLast(spawnPoint);

            NextDirection = direction;
            ChangeDirection(direction);

            Color = color;
        }

        /// <summary>
        /// Resets the snake by shrinking it back to its initial starting size and moving it to
        /// a random suitable location in the arena.
        /// </summary>
        public void Reset()
        {
            // Clear all cells occupied by this snake in the arena
            foreach(var p in Body)
                Arena.UpdateCell(p.X, p.Y, null);

            Body = new LinkedList<Point>();
            Growth = 2;

            var spawnPoint = Arena.GetSpawnPoint();
            Body.AddLast(spawnPoint);

            var direction = Direction.Right;
            NextDirection = direction;
            ChangeDirection(direction);
        }

        public void Move()
        {
            var newHead = GetNewHead();

            // If an object on the ground exists, interact with it.
            Arena.GetCell(newHead.X, newHead.Y)?.Interact(this);

            // Set the currently visited cell as the player's.
            Arena.UpdateCell(newHead.X, newHead.Y, new SnekBody(Color, this, Body.AddLast(newHead)));

            // Free the tail cell if snake is not growing.
            var tail = Body.First.Value;
            if (Growth > 0)
                Growth--;
            else
            {
                Arena.UpdateCell(tail.X, tail.Y, null);
                Body.RemoveFirst();
            }

            CurrentDirection = NextDirection;
        }

        public void ChangeDirection(Direction newDirection)
        {
            switch (newDirection)
            {
                case Direction.Up:
                    if (CurrentDirection != Direction.Down)
                    {
                        _speed.X = 0;
                        _speed.Y = -1;
                        NextDirection = newDirection;
                    }
                    break;

                case Direction.Down:
                    if (CurrentDirection != Direction.Up)
                    {
                        _speed.X = 0;
                        _speed.Y = 1;
                        NextDirection = newDirection;
                    }
                    break;

                case Direction.Left:
                    if (CurrentDirection != Direction.Right)
                    {
                        _speed.X = -1;
                        _speed.Y = 0;
                        NextDirection = newDirection;
                    }
                    break;

                case Direction.Right:
                    if (CurrentDirection != Direction.Left)
                    {
                        _speed.X = 1;
                        _speed.Y = 0;
                        NextDirection = newDirection;
                    }
                    break;
                default:
                    _speed.X = 0;
                    _speed.Y = 0;
                    NextDirection = Direction.None;
                    break;
            }

            _speed.X *= SpeedModifier;
            _speed.Y *= SpeedModifier;
        }

        private Point GetNewHead()
        {
            var head = Body.Last.Value;

            var newX = (head.X + _speed.X) % Arena.Width;
            if (newX < 0)
            {
                newX += Arena.Width;
            }

            var newY = (head.Y + _speed.Y) % Arena.Height;
            if (newY < 0)
            {
                newY += Arena.Height;
            }

            var newHead = new Point(newX, newY);
            return newHead;
        }

        public void TrimTail(LinkedListNode<Point> cutoffPoint)
        {
            // Trim the list until snake was eaten
            while (Body.First != cutoffPoint && Body.Last?.Previous != null)
            {
                var cell = Body.First.Value;
                Body.RemoveFirst();
                Arena.UpdateCell(cell.X, cell.Y, null);
            }

            // Trim it once more, because it was trimmed up until the trim point. Exempt if it would obliterate the snake.
            if (Body.Last?.Previous != null)
            {
                var cell = Body.First.Value;
                Body.RemoveFirst();
                Arena.UpdateCell(cell.X, cell.Y, null);
            }
        }

        public object Clone()
        {
            var cpy = MemberwiseClone() as Snek;

            // Deep copy the body
            cpy.Body = new LinkedList<Point>(Body);

            return cpy;
        }
    }
}
