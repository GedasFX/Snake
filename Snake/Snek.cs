using System;
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
        public LinkedList<Point> Body = new LinkedList<Point>();

        private Direction _direction;
        private readonly Arena _arena;

        private int _growth = 2;
        private Point _speed;

        public Snek(Arena arena, Point spawnPoint)
        {
            _arena = arena;

            Body.AddLast(spawnPoint);
            ChangeDirection(Direction.Right);
        }

        public void Move()
        {
            var newHead = GetNewHead();

            // If an object on the ground exists, interact with it.
            _arena.Cells[newHead.X, newHead.Y]?.Interact(this);

            // Set the currently visited cell as the player's.
            _arena.Cells[newHead.X, newHead.Y] = new SnekBody(Body.AddLast(newHead));

            // Free the tail cell if snake is not growing.
            var tail = Body.First.Value;
            if (_growth > 0)
                _growth--;
            else
            {
                _arena.Cells[tail.X, tail.Y] = null;
                Body.RemoveFirst();
            }
        }

        public void AddGrowth(int deltaGrowth) => _growth += deltaGrowth;

        public void ChangeDirection(Direction newDirection)
        {
            switch (newDirection)
            {
                case Direction.Up:
                    if (_direction != Direction.Down)
                    {
                        _speed.X = 0;
                        _speed.Y = -1;
                        _direction = newDirection;
                    }
                    break;

                case Direction.Down:
                    if (_direction != Direction.Up)
                    {
                        _speed.X = 0;
                        _speed.Y = 1;
                        _direction = newDirection;
                    }
                    break;

                case Direction.Left:
                    if (_direction != Direction.Right)
                    {
                        _speed.X = -1;
                        _speed.Y = 0;
                        _direction = newDirection;
                    }
                    break;

                case Direction.Right:
                    if (_direction != Direction.Left)
                    {
                        _speed.X = 1;
                        _speed.Y = 0;
                        _direction = newDirection;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newDirection), newDirection, null);
            }
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
    }
}
