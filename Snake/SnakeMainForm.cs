using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Snake
{
    public partial class SnakeMainForm : Form
    {
        private Dictionary<Point, Color> _map = new Dictionary<Point, Color>();
        private Direction _nextDirection;

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

                        await Console.Out.WriteLineAsync($"Received {res.Count} bytes");

                        _map = JsonConvert.DeserializeObject<Message>(msg).Arena;

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
