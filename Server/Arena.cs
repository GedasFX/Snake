using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Server.ArenaItems;

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

        public Arena()
        {
            Cells = new ICell[Width, Height];
            ColorMap = new Dictionary<Point, Color>();

            Players = new List<Player>();
        }


        public void AddConnection(WebSocket socket)
        {
            Players.Add(new Player(socket, this));
        }

        public Task StartAsync()
        {
            return Task.Run(async () =>
            {
                var cycle = 0;
                while (true)
                {
                    Console.WriteLine(cycle++);
                    foreach (var p in Players)
                    {
                        // Serialize the arena
                        var obj = new Message { Arena = ColorMap };
                        var ser = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));

                        // Send the message
                        var ct = new CancellationTokenSource();
                        ct.CancelAfter(250);

                        await p.Socket.SendAsync(new ArraySegment<byte>(ser), WebSocketMessageType.Text,
                            true, ct.Token);

                        p.Snake.Move();
                    }

                    if (_random.Next(100) <= 4)
                    {
                        CreateFood();
                    }

                    await Task.Delay(100);
                }

                // ReSharper disable once FunctionNeverReturns
            });
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
    }
}