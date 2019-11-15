using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ConsoleColor = System.ConsoleColor;

namespace Snake
{
    public partial class SnakeMainForm : Form
    {
        private Dictionary<Point, Color> _map = new Dictionary<Point, Color>();
        private Direction _nextDirection;

        // When the client first joins the game, the previous state of the game is unknown.
        private GameStateEnum _previousState = GameStateEnum.Unknown;

        public SnakeMainForm()
        {
            InitializeComponent();
            JoinArena().ConfigureAwait(false);
        }

        private void PanelArena_Paint(object sender, PaintEventArgs e)
        {
            ArenaView.Render(e.Graphics, _map, 40, 50);
        }

        private async Task JoinArena()
        {
            using (var socket = new ClientWebSocket())
            {
                try
                {
                    await socket.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
                }
                catch (Exception e)
                {
                    _ = Console.Out.WriteLineAsync(e.ToString());
                    throw;
                }

                while (true)
                {
                    try
                    {
                        var buf = new byte[4096];

                        var msg = "";
                        WebSocketReceiveResult res;
                        do
                        {
                            res = await socket.ReceiveAsync(new ArraySegment<byte>(buf), CancellationToken.None);
                            msg += Encoding.ASCII.GetString(buf, 0, res.Count);
                        } while (!res.EndOfMessage);

                        // await Console.Out.WriteLineAsync($"Received {res.Count} bytes");

                        var message = JsonConvert.DeserializeObject<Message>(msg);
                        WriteGameInfo(message);
                        _map = message.Arena;

                        PanelArena.Refresh();

                        await socket.SendAsync(new ArraySegment<byte>(new[] { (byte)_nextDirection }),
                            WebSocketMessageType.Binary, true, CancellationToken.None);

                    }
                    catch (Exception e)
                    {
                        _ = Console.Out.WriteLineAsync(e.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Writes some information about the game to the console. When the game finishes, the player
        /// standings of the top three players are written as well.
        /// </summary>
        /// <param name="message">Message received from the server</param>
        private void WriteGameInfo(Message message)
        {
            var currentState = message.GameState;

            // Don't spam the console if the game state hasn't changed yet.
            if (currentState != _previousState)
            {
                switch (currentState)
                {
                    case GameStateEnum.Pregame:
                        WriteLineWithColor(ConsoleColor.Green, "The game is about to start!");
                        break;
                    case GameStateEnum.InProgress:
                        WriteLineWithColor(ConsoleColor.Green, "The game is in progress!");
                        break;
                    case GameStateEnum.EndingSoon:
                        WriteLineWithColor(ConsoleColor.Yellow, "The game is about to end soon!");
                        break;
                    case GameStateEnum.PostGame:
                        WriteLineWithColor(ConsoleColor.Red, "The game has ended!");
                        WriteStandings(message.Podium);
                        break;
                    case GameStateEnum.Unknown:
                        throw new ApplicationException("Received an unknown game state?");
                }
            }

            _previousState = currentState;
        }

        /// <summary>
        /// Writes the player standings to the console.
        /// </summary>
        /// <param name="playerStandingsData">Player standings array</param>
        private void WriteStandings(PlayerStandingsData[] playerStandingsData)
        {
            // The standings are a bit crap because the colors aren't particularly human readable, but whatever.
            WriteLineWithColor(ConsoleColor.Green, "====    PLAYER STANDINGS    ====");
            WriteLineWithColor(ConsoleColor.Green, "==== Sorted by snake length ====");

            foreach (var element in playerStandingsData)
            {
                string line = $"Player (color: {element.PlayerColor.ToString()}): {element.SnakeLength}";
                WriteLineWithColor(ConsoleColor.Green, line);
            }
        }

        private void WriteLineWithColor(ConsoleColor color, string msg)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        private void SnakeMainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    _nextDirection = Direction.Down;
                    break;

                case Keys.Left:
                    _nextDirection = Direction.Left;
                    break;

                case Keys.Right:
                    _nextDirection = Direction.Right;
                    break;

                case Keys.Up:
                    _nextDirection = Direction.Up;
                    break;
            }
        }
    }
}
