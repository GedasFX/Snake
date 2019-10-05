using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Server.ArenaItems;
using Server.Strategies.FoodSpawning;

namespace Server
{
    public class Arena
    {
        public List<Player> Players { get; }

        public int Width { get; } = 50;
        public int Height { get; } = 40;

        public ICell[,] Cells { get; }
        public Dictionary<Point, Color> ColorMap { get; }

        private readonly Random _random = new Random(0);

        // TODO: Refactor
        private  IFoodSpawningStrategy randomStrategy, plusStrategy, squareStrategy;

        private IFoodSpawningStrategy currentStrategy;

        public Arena()
        {
            Cells = new ICell[Width, Height];
            ColorMap = new Dictionary<Point, Color>();

            // Strategies
            randomStrategy = new FoodSpawningRandomStrategy(5, _random);
            plusStrategy = new FoodSpawningPlusStrategy(5, _random);
            squareStrategy = new FoodSpawningSquareStrategy(3, _random);

            currentStrategy = randomStrategy;

            Players = new List<Player>();
        }


        public void AddConnection(WebSocket socket, TaskCompletionSource<object> playerDisconnected)
        {
            Players.Add(new Player(socket, playerDisconnected, this,
                Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255))));
        }

        private void RemoveConnection(Player player)
        {
            Players.Remove(player);
            foreach (var point in player.Snake.Body)
            {
                UpdateCell(point.X, point.Y, null);
            }

            player.PlayerDisconnected.TrySetResult(null);
        }

        public async Task StartAsync()
        {
            var cycle = 0;
            while (true)
            {
                try
                {
                    // Print the number of ticks elapsed.
                    Logger.Instance.LogMessage($"Number of ticks elapsed: {cycle++}");

                    // Update every player
                    Logger.Instance.LogMessage($"Updating {Players.Count} player(s)");
                    foreach (var p in Players)
                    {
                        // Serialize the arena
                        var obj = new Message { Arena = ColorMap };
                        var ser = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));

                        // Send the message
                        var ct = new CancellationTokenSource();
                        ct.CancelAfter(250);

                        var task = p.Socket.SendAsync(new ArraySegment<byte>(ser), WebSocketMessageType.Text,
                            true, ct.Token);
                        if (p.CheckDisconnected(task))
                        {
                            RemoveConnection(p);
                            continue;
                        }

                        p.Snake.Move();
                    }


                    // Generate food.
                    Logger.Instance.LogMessage("Generating food ...");
                    if (_random.Next(20) == 0)
                    {
                        currentStrategy.Spawn(this);
                        SwitchFoodGenerationStrategy();
                    }

                    // Wait until next server tick.
                    Logger.Instance.LogMessage("Waiting until next tick ...");
                    await Task.Delay(100);
                }
                catch (Exception e)
                {
                    Logger.Instance.LogError(e.StackTrace);
                }
            }
        }

        private void SwitchFoodGenerationStrategy()
        {
            // Log.Instance.LogMessage("Switching strategy!");
            Console.WriteLine("Switching strategy!");
            int num = _random.Next(3);
            switch(num)
            {
                case 0:
                    currentStrategy = randomStrategy;
                    Console.WriteLine("Switched to random food generation strategy!");
                    break;
                case 1:
                    currentStrategy = plusStrategy;
                    Console.WriteLine("Switched to plus pattern food generation strategy!");
                    break;
                case 2:
                    currentStrategy = squareStrategy;
                    Console.WriteLine("Switched to square pattern food generation strategy!");
                    break;
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
    }
}