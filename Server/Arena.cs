using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Server.ArenaItems;
using Server.Facades;
using Server.Strategies.FoodSpawning;

namespace Server
{
    public class Arena : IObservable<Message>
    {
        public static Arena Instance { get; } = new Arena();

        public List<Player> Players { get; } = new List<Player>();

        public int Width { get; } = 50;
        public int Height { get; } = 40;

        private ICell[,] Cells { get; }
        private Dictionary<Point, Color> ColorMap { get; }

        private readonly Random _random = new Random(0);

        private readonly FoodSpawningFacade _foodSpawningFacade;

        private Arena()
        {
            Cells = new ICell[Width, Height];
            ColorMap = new Dictionary<Point, Color>();

            // Create food spawning facade
            _foodSpawningFacade = new FoodSpawningFacadeAdapter(this, _random) as FoodSpawningFacade;
        }


        public void Connect(WebSocket socket, TaskCompletionSource<object> playerDisconnected)
        {
            Subscribe(new Player(socket, playerDisconnected, this,
                Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255))));
        }

        public async Task StartAsync()
        {
            while (true)
            {
                try
                {
                    // Print the number of ticks elapsed.
                    // Logger.Instance.LogMessage($"Number of ticks elapsed: {cycle++}");

                    // Update every player
                    // Logger.Instance.LogMessage($"Updating {Players.Count} player(s)");

                    var message = new Message { Arena = ColorMap };
                    foreach (var p in Players)
                    {
                        p.OnNext(message);
                    }

                    // Generate food.
                    _foodSpawningFacade.ExecuteTick();

                    // Wait until next server tick.
                    // Logger.Instance.LogMessage("Waiting until next tick ...");
                    await Task.Delay(100);
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
            UpdateCell(_random.Next(0, Width), _random.Next(0, Height), FoodFactory.GenerateFoodItem());
        }

        public void CreateFood(int x, int y)
        {
            UpdateCell(x, y, FoodFactory.GenerateFoodItem());
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