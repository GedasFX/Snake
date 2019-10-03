using Server.ArenaItems;
using System;

namespace Server.Strategies.FoodSpawning
{
    /// <summary>
    /// Strategy which spawns food scattered around randomly.
    /// </summary>
    public class FoodSpawningRandomStrategy : IFoodSpawningStrategy
    {
        /// <summary>
        /// Constructs a strategy which spawns a number of food items scattered around randomly.
        /// </summary>
        /// <param name="spawnCount">Number of food items to spawn.</param>
        /// <param name="rng">Reference to random number generator.</param>
        public FoodSpawningRandomStrategy(int spawnCount, Random rng)
        {
            _spawnCount = spawnCount;
            _rng = rng;
        }

        public void Spawn(Arena arena)
        {
            for(int i = 0; i < _spawnCount; ++i)
            {
                int y, x;
                ICell cell;

                // Generate random points in the arena until we find an unoccupied one.
                do
                {
                    y = _rng.Next(arena.Height);
                    x = _rng.Next(arena.Width);
                    cell = arena.GetCell(x, y);
                } while (cell != null);

                arena.CreateFood(x, y);
            }
        }

        private readonly int _spawnCount;       // Number of food items to spawn in the arena
        private readonly Random _rng;           // Random number generator

    }
}
