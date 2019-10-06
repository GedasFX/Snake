﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Server.ArenaItems;
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

        private readonly List<IFoodSpawningStrategy> _strategies = new List<IFoodSpawningStrategy>();

        private IFoodSpawningStrategy _currentStrategy;

        private Arena()
        {
            Cells = new ICell[Width, Height];
            ColorMap = new Dictionary<Point, Color>();

            // Strategies
            _strategies.Add(new FoodSpawningRandomStrategy(5, _random));
            _strategies.Add(new FoodSpawningPlusStrategy(5, _random));
            _strategies.Add(new FoodSpawningSquareStrategy(3, _random));
            _currentStrategy = _strategies[0];
        }


        public void Connect(WebSocket socket, TaskCompletionSource<object> playerDisconnected)
        {
            Subscribe(new Player(socket, playerDisconnected, this,
                Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255))));
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

                    var message = new Message { Arena = ColorMap };
                    foreach (var p in Players)
                    {
                        p.OnNext(message);
                    }

                    // Generate food.
                    Logger.Instance.LogMessage("Generating food ...");
                    if (_random.Next(20) == 0)
                    {
                        _currentStrategy.Spawn(this);
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
            var num = _random.Next(3);

            _currentStrategy = _strategies[num];

            switch (num)
            {
                case 0:
                    Logger.Instance.LogMessage("Switched to random food generation strategy!");
                    break;
                case 1:
                    Logger.Instance.LogMessage("Switched to plus pattern food generation strategy!");
                    break;
                case 2:
                    Logger.Instance.LogMessage("Switched to square pattern food generation strategy!");
                    break;
                default:
                    throw new ArgumentException();
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