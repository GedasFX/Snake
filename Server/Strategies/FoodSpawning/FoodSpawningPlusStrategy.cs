using Server.ArenaItems;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Strategies.FoodSpawning
{
    /// <summary>
    /// Strategy that spawns food in plus patterns at random locations.
    /// </summary>
    public class FoodSpawningPlusStrategy : IFoodSpawningStrategy
    {
        /// <summary>
        /// Constructs a strategy which spawns food in a plus pattern at random locations. Warning: not tested, may be buggy.
        /// </summary>
        /// <param name="lineLength">Length of the two lines that make up the plus (must be an odd number).</param>
        /// <param name="rng">Random number generator.</param>
        public FoodSpawningPlusStrategy(int lineLength, Random rng)
        {
            if (lineLength % 2 == 0)
                throw new ArgumentException("The lengths of the lines that make up the plus must be odd!");

            _lineLength = lineLength;
            _rng = rng;
        }

        public void Spawn(Arena arena)
        {
            int spikeLength = _lineLength / 2;      // Lengths of the "spikes" that stick out of the center.

            int centerX, centerY;                   // Coordinates of the center of the plus.
            List<ICell> cellsInPlus;                // Cells that make up the plus.

            do
            {
                centerY = _rng.Next(spikeLength, arena.Height - spikeLength);
                centerX = _rng.Next(spikeLength, arena.Width - spikeLength);
                cellsInPlus = CollectCells(centerX, centerY, spikeLength, arena);
            } while (!cellsInPlus.TrueForAll(c => c == null));

            // Collect points
            int minX = centerX - spikeLength;
            int maxX = centerX + spikeLength;
            int minY = centerY - spikeLength;
            int maxY = centerY + spikeLength;

            var points = new List<Point>();
            // Collect horizontal line first
            for (int x = minX; x <= maxX; ++x)
                points.Add(new Point(x, centerY));

            // Collect vertical line (skipping the center)
            for (int y = minY; y <= maxY; ++y)
                if (y != centerY)
                    points.Add(new Point(centerX, y));

            foreach (var point in points)
                arena.CreateFood(point.X, point.Y);
        }

        private List<ICell> CollectCells(int centerX, int centerY, int spikeLength, Arena arena)
        {
            var cells = new List<ICell>();

            int minX = centerX - spikeLength;
            int maxX = centerX + spikeLength;
            int minY = centerY - spikeLength;
            int maxY = centerY + spikeLength;

            // Collect horizontal line first
            for (int x = minX; x <= maxX; ++x)
                cells.Add(arena.GetCell(x, centerY));

            // Collect vertical line (skipping the center)
            for (int y = minY; y <= maxY; ++y)
                if (y != centerY)
                    cells.Add(arena.GetCell(centerX, y));

            return cells;
        }

        private readonly int _lineLength;           // Length of the two lines that make up the plus.
        private readonly Random _rng;
    }
}
