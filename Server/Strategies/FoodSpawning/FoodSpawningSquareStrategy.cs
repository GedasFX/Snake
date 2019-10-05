using Server.ArenaItems;
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
            int topLeftY, topLeftX;             // Coordinates of the top left point of the square
            List<ICell> cellsInSquare;          // List of all cells that comprise the square

            do
            {
                topLeftY = _rng.Next(arena.Height - _sideLength + 1);
                topLeftX = _rng.Next(arena.Width - _sideLength + 1);
                cellsInSquare = CollectCells(topLeftY, topLeftX, arena);
            } while (!cellsInSquare.TrueForAll(c => c == null));

            // Found suitable location, calculate bottom right point and collect all points that comprise the square.
            var bottomRight = new Point(topLeftX + _sideLength - 1, topLeftY + _sideLength - 1);
            var points = new List<Point>();

            for (int y = topLeftY; y < bottomRight.Y; ++y)
                for (int x = topLeftX; x < bottomRight.X; ++x)
                    points.Add(new Point(x, y));

            foreach (var point in points)
                arena.CreateFood(point.X, point.Y);
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

        private readonly int _sideLength;       // Length of the squares' sides
        private readonly Random _rng;           // Random number generator
    }
}
