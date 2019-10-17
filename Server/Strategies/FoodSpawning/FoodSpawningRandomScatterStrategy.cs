using Server.ArenaItems;
using System;

namespace Server.Strategies.FoodSpawning
{
    /// <summary>
    /// Strategy which spawns food scattered around randomly.
    /// </summary>
    public class FoodSpawningRandomScatterStrategy : IFoodSpawningStrategy
    {
        /// <summary>
        /// Constructs a strategy which spawns a number of food items scattered around randomly.
        /// </summary>
        /// <param name="spawnAmount">Number of food items to spawn.</param>
        /// <param name="rng">Reference to random number generator.</param>
        public FoodSpawningRandomScatterStrategy(int spawnAmount, Random rng)
        {
            _spawnAmount = spawnAmount;
            _rng = rng;
        }

        public void Spawn(Arena arena)
        {
            for(int i = 0; i < _spawnAmount; ++i)
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

        /// <summary>
        /// Sets the amount of food items that are spawned for this strategy.
        /// </summary>
        /// <param name="amount"></param>
        public void SetSpawnAmount(int amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Spawn count must be positive and non-zero!");

            _spawnAmount = amount;
        }

        private int _spawnAmount;       // Number of food items to spawn in the arena
        private readonly Random _rng;           // Random number generator

    }
}
