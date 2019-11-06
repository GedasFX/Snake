using System;
using Server.Strategies.FoodSpawning;

namespace Server.Facades
{
    /// <summary>
    /// A facade which simplifies the food spawning process: it provides sensible defaults for the spawning strategies,
    /// allows to set the individual strategies' parameters, provides an easy way of switching between them (including randomly),
    /// it also allows the user of the facade to change the frequency at which the food spawns at (+ spawn chance)
    /// </summary>
    public class FoodSpawningFacade
    {
        /// <summary>
        /// <p>Constructs a food spawning facade which simplifies the process of food spawning. It sets some defaults
        /// for the various strategies that are available to use. It also:</p>
        /// <list type="bullet">
        /// <item>
        /// <description>Sets the default food spawning strategy to random scattering.</description>
        /// </item>
        /// <item>
        /// <description>Sets the frequency at which the food may potentially spawn at to 20 ticks.</description>
        /// </item>
        /// <item>
        /// <description>Sets the chance for food to spawn during the spawn tick at 5% chance.</description>
        /// </item>
        /// </list>
        /// All these options may be later modified by the user.
        /// </summary>
        /// <param name="arena">Arena where food will be spawned</param>
        /// <param name="rng">Random number generator to use during the spawning process</param>
        public FoodSpawningFacade(Arena arena, Random rng)
        {
            _arena = arena;
            _rng = rng;

            // Construct strategies, provide some defaults.
            _randomScatterScatterStrategy = new FoodSpawningRandomScatterStrategy(5, _rng);
            _squareStrategy = new FoodSpawningSquareStrategy(2, _rng);
            _plusStrategy = new FoodSpawningPlusStrategy(5, _rng);

            // Set the default chosen strategy to random scattering.
            _currentStrategy = _randomScatterScatterStrategy;

            // Set spawning frequency at 20 ticks.
            SpawnFrequency = 20;

            // Set chance for food spawn during spawn ticks to 5%
            SpawnChance = 50.0;

            // Notify with message to console.
            Logger.Instance.LogMessage("Food spawning facade constructed!");
        }

        #region Arena & RNG variables

        // Arena where food items will be spawned in.
        private readonly Arena _arena;

        // Random number generator used during the food spawning process.
        private readonly Random _rng;

        #endregion

        #region Strategy variables

        // Available strategies to use.
        private FoodSpawningRandomScatterStrategy _randomScatterScatterStrategy;
        private FoodSpawningSquareStrategy _squareStrategy;
        private FoodSpawningPlusStrategy _plusStrategy;

        // Currently chosen strategy.
        private IFoodSpawningStrategy _currentStrategy;

        #endregion

        #region Food spawner variables

        private int _spawnFrequency;

        /// <summary>
        /// The frequency (in ticks) at which food items may spawn in the arena (it depends on spawn chance also)
        /// </summary>
        public int SpawnFrequency
        {
            get => _spawnFrequency;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Spawn frequency cannot be zero or negative!");

                _spawnFrequency = value;
            }
        }

        private double _spawnChance;

        /// <summary>
        /// The percentage chance that food may spawn during the spawn tick. 
        /// </summary>
        public double SpawnChance
        {
            get => _spawnChance * 100.0; // Convert from range [0.0; 1.0] to range [0.0; 100.0]
            set
            {
                if (value < 0.0 || value > 100.0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Spawn chance must be between 0 and 100%!");

                _spawnChance = value / 100.0; // Convert from range [0.0; 100.0] to range [0.0; 1.0]
            }
        }

        /// <summary>
        /// Current tick of the food spawner.
        /// </summary>
        public int CurrentTick { get; private set; } = 0;

        #endregion

        #region Methods
        
        /// <summary>
        /// Executes a tick of the food spawning process. If this tick is a spawn tick, the spawner may spawn some food.
        /// This function should be called every tick.
        /// </summary>
        public virtual void ExecuteTick()
        {
            // Check if the current tick is a spawn tick.
            if (CurrentTick % SpawnFrequency == 0)
            {
                Logger.Instance.LogMessage($"Food spawning facade: current tick {CurrentTick} is spawn tick!");
                // Roll for chance to spawn food.
                double roll = _rng.NextDouble() * 100.0;
                if (roll < SpawnChance)
                {
                    Logger.Instance.LogMessage($"Food spawning facade: roll successful! Spawning food...");
                        _currentStrategy.Spawn(_arena);
                }
                else
                {
                    Logger.Instance.LogMessage($"Food spawning facade: roll unsuccessful!");
                }

                if (roll > 95)
                {
                    SwitchStrategyAtRandom();
                }
            }

            CurrentTick++;
        }

        /// <summary>
        /// Switches to the random scatter food strategy.
        /// </summary>
        public void SwitchToRandomScatterStrategy()
        {
            Logger.Instance.LogMessage("Food spawning facade: switching to random scatter strategy!");
            _currentStrategy = _randomScatterScatterStrategy;
        }

        /// <summary>
        /// Switches to the strategy which generates food in squares.
        /// </summary>
        public void SwitchToSquareStrategy()
        {
            Logger.Instance.LogMessage("Food spawning facade: switching to food square strategy!");
            _currentStrategy = _squareStrategy;
        }

        /// <summary>
        /// Switches to the strategy which generates food in pluses.
        /// </summary>
        public void SwitchToPlusStrategy()
        {
            Logger.Instance.LogMessage("Food spawning facade: switching to plus generation strategy!");
            _currentStrategy = _plusStrategy;
        }

        /// <summary>
        /// Picks a random strategy (random scatter, squares, pluses) and switches to it.
        /// </summary>
        public virtual void SwitchStrategyAtRandom()
        {
            int roll = _rng.Next(3);

            switch (roll)
            {
                case 0:
                    SwitchToRandomScatterStrategy();
                    break;
                case 1:
                    SwitchToSquareStrategy();
                    break;
                case 2:
                    SwitchToPlusStrategy();
                    break;
            }
        }

        /// <summary>
        /// Sets the amount of food items that are spawned in one go if the selected strategy is random scatter.
        /// </summary>
        /// <param name="amount">Number of food items to spawn</param>
        public void SetRandomScatterSpawnAmount(int amount)
        {
            try
            {
                _randomScatterScatterStrategy.SetSpawnAmount(amount);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Logger.Instance.LogError(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Sets the size of the food squares that are generated if the square generation strategy is selected.
        /// </summary>
        /// <param name="sideLength">Length of the sides of the square</param>
        public void SetFoodSquareSize(int sideLength)
        {
            try
            {
                _squareStrategy.SetSideLength(sideLength);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Logger.Instance.LogError(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Sets the size of the pluses that are generated if the plus generation strategy is selected.
        /// </summary>
        /// <param name="length">Length of the lines that make up the plus.</param>
        public void SetPlusSize(int length)
        {
            try
            {
                _plusStrategy.SetLineLength(length);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Logger.Instance.LogError(ex.StackTrace);
                throw;
            }
        }

        #endregion
    }
}
