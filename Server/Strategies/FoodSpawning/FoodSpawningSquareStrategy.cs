﻿using Server.ArenaItems;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Server.Strategies.FoodSpawning
{
    /// <summary>
    /// Strategy which spawns food in squares at random locations.
    /// </summary>
    public class FoodSpawningSquareStrategy : IFoodSpawningStrategy
    {
        /// <summary>
        /// Constructs a strategy which spawns food in squares at random locations. Warning: not tested, may be buggy.
        /// </summary>
        /// <param name="sideLength">Length of the squares' sides (i.e. the size).</param>
        /// <param name="rng">Reference to random number generator.</param>
        public FoodSpawningSquareStrategy(int sideLength, Random rng)
        {
            _sideLength = sideLength;
            _rng = rng;
        }

        public void Spawn(Arena arena)
        {
            var topLeftY = _rng.Next(arena.Height - _sideLength + 1);
            var topLeftX = _rng.Next(arena.Width - _sideLength + 1);

            var cellsInSquare = CollectCells(topLeftY, topLeftX, arena);
            if (cellsInSquare.Any(c => c is ISnekBody))
                return; // Spawn Failed

            // Found suitable location, calculate bottom right point and collect all points that comprise the square.
            var bottomRight = new Point(topLeftX + _sideLength - 1, topLeftY + _sideLength - 1);
            var points = new List<Point>();

            for (int y = topLeftY; y < bottomRight.Y; ++y)
                for (int x = topLeftX; x < bottomRight.X; ++x)
                    points.Add(new Point(x, y));

            foreach (var point in points)
                arena.CreateFood(point.X, point.Y);
        }

        /// <summary>
        /// Sets the length of the sides of the generated squares
        /// </summary>
        /// <param name="length">Length of the sides</param>
        public void SetSideLength(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Side length must be positive and non-zero!");

            _sideLength = length;
        }

        private List<ICell> CollectCells(int topLeftY, int topLeftX, Arena arena)
        {
            // Calculate bottom right point location
            var bottomRight = new Point(topLeftX + _sideLength - 1, topLeftY + _sideLength - 1);

            var cells = new List<ICell>();
            for (int y = topLeftY; y <= bottomRight.Y; ++y)
                for (int x = topLeftX; x <= bottomRight.X; ++x)
                    cells.Add(arena.GetCell(x, y));
            return cells;
        }

        private int _sideLength;       // Length of the squares' sides
        private readonly Random _rng;           // Random number generator
    }
}
