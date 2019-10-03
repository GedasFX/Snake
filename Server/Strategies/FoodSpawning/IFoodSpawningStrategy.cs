namespace Server.Strategies.FoodSpawning
{
    /// <summary>
    /// An interface for various food spawning strategies.
    /// </summary>
    interface IFoodSpawningStrategy
    {
        /// <summary>
        /// Spawns food in the arena according to a concrete strategy.
        /// </summary>
        /// <param name="arena">Reference to arena where food will be spawned in.</param>
        void Spawn(Arena arena);
    }
}
