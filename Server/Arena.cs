using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Server.ArenaItems;
using Server.Facades;
using Server.GameStates;

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
        public GameStateContext GameStateContext { get; }

        protected Arena()
        {
            Cells = new ICell[Width, Height];
            ColorMap = new Dictionary<Point, Color>();

            // Create food spawning facade
            FoodSpawningFacade = new FoodSpawningFacadeAdapter(this, _random);

            GameStateContext = new GameStateContext(this);
        }


        public void Connect(WebSocket socket, TaskCompletionSource<object> playerDisconnected)
        {
            var playerColor = Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255));
            var player = new Player(socket, playerDisconnected, this, playerColor);
            Subscribe(player);
        }

        public virtual async Task StartAsync()
        {
            while (true)
            {
                try
                {
                    Message message;
                    var currentStateOfGame = GameStateContext.GetStateOfGameAsEnum();
                    if(currentStateOfGame != GameStateEnum.PostGame)
                        message = new Message(ColorMap, currentStateOfGame, null);
                    else
                    {
                        // Only send podium data when the game has finished.
                        var podium = GetPlayerStandings().Take(3).ToArray();
                        message = new Message(ColorMap, currentStateOfGame, podium);
                    }

                    foreach (var p in Players)
                        p.OnNext(message);

                    // Run game
                    GameStateContext.Run();

                    await Task.Delay(250);
                }
                catch (Exception e)
                {
                    Logger.Instance.LogError(e.StackTrace);
                }
            }
        }

        /// <summary>
        /// Gets standings for all players in the arena. The standings are sorted in descending order by snake lengths.
        /// </summary>
        /// <returns>An array of PlayerStandingsData objects</returns>
        public IEnumerable<PlayerStandingsData> GetPlayerStandings()
        {
            var standings = Players.Select(p => new PlayerStandingsData(p.Snake.Color, p.Snake.Body.Count))
                .OrderByDescending(st => st.SnakeLength).ToArray();
            return standings;
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

        /// <summary>
        /// Resets the arena, preparing it for a new game.
        /// </summary>
        public void Reset()
        {
            for (int x = 0; x < Width; ++x)
                for (int y = 0; y < Height; ++y)
                    Cells[x, y] = null;

            ColorMap.Clear();

            foreach(var p in Players)
                p.ResetSnake();
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