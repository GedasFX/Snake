using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Server.ArenaItems;
using Server.Facades;

namespace Server
{
    public class Arena : IObservable<Message>
    {
        public List<Player> Players { get; } = new List<Player>();

        public int Width { get; } = 50;
        public int Height { get; } = 40;

        private ICell[,] Cells { get; }
        public Dictionary<Point, Color> ColorMap { get; }

        private readonly Random _random = new Random(0);

        public FoodSpawningFacade FoodSpawningFacade { get; }

        protected Arena()
        {
            Cells = new ICell[Width, Height];
            ColorMap = new Dictionary<Point, Color>();

            // Create food spawning facade
            FoodSpawningFacade = new FoodSpawningFacadeAdapter(this, _random);
        }


        public void Connect(WebSocket socket, TaskCompletionSource<object> playerDisconnected)
        {
            Subscribe(new Player(socket, playerDisconnected, this,
                Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255))));
        }

        public virtual async Task StartAsync()
        {
            while (true)
            {
                try
                {
                    // Print the number of ticks elapsed.
                    // Logger.Instance.LogMessage($"Number of ticks elapsed: {cycle++}");

                    // Update every player
                    // Logger.Instance.LogMessage($"Updating {Players.Count} player(s)");

                    var message = new Message(ColorMap);
                    foreach (var p in Players)
                    {
                        p.OnNext(message);
                    }

                    // Generate food.
                    FoodSpawningFacade.ExecuteTick();

                    // Wait until next server tick.
                    // Logger.Instance.LogMessage("Waiting until next tick ...");
                    await Task.Delay(250);
                }
                catch (Exception e)
                {
                    Logger.Instance.LogError(e.StackTrace);
                }
            }
        }

        public ICell GetCell(int x, int y) => Cells[x, y];

        public void UpdateCell(int x, int y, ICell value)
        {
            Cells[x, y] = value;

            var point = new Point(x, y);

            if (value == null)
                ColorMap.Remove(point);
            else
                ColorMap[point] = value.Color;
        }

        public void CreateFood()
        {
            CreateFood(_random.Next(0, Width), _random.Next(0, Height));
        }

        public void CreateFood(int x, int y)
        {
            UpdateCell(x, y, FoodFactory.GenerateFoodItem());
        }

        /// <summary>
        /// Gets a point suitable for spawning a snake.
        /// </summary>
        /// <returns>A spawn point</returns>
        public Point GetSpawnPoint()
        {
            IList<Point> emptyPoints = GetEmptyPoints();
            if (emptyPoints.Count == 0)
            {
                // Arena is completely filled, forcibly remove a random food item from the arena and spawn the snake there.
                Point spawnPoint = GetRandomFoodItemLocation();
                Cells[spawnPoint.X, spawnPoint.Y] = null;
                return spawnPoint;
            }

            int index = _random.Next(emptyPoints.Count);
            return emptyPoints[index];
        }

        /// <summary>
        /// Gets a list of empty points in the arena.
        /// </summary>
        /// <returns>A list of empty points</returns>
        private IList<Point> GetEmptyPoints()
        {
            var emptyPoints = new List<Point>();

            for (int x = 0; x < Cells.GetLength(0); ++x)
            {
                for (int y = 0; y < Cells.GetLength(1); ++y)
                {
                    if(Cells[x, y] == null)
                        emptyPoints.Add(new Point(x, y));
                }
            }

            return emptyPoints;
        }

        /// <summary>
        /// Gets the location of a random food item in the arena.
        /// </summary>
        /// <returns>The location of the food item</returns>
        private Point GetRandomFoodItemLocation()
        {
            var p = new Point();
            ICell selectedCell;
            int x, y;
            do
            {
                x = _random.Next(Width);
                y = _random.Next(Height);
                selectedCell = Cells[x, y];
            } while (!(selectedCell is IFoodItem));

            p.X = x;
            p.Y = y;
            return p;
        }

        public IDisposable Subscribe(IObserver<Message> observer)
        {
            if (!(observer is Player player))
                throw new ArgumentException();

            Players.Add(player);

            return player;
        }
    }
}