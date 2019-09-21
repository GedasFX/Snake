using System.Drawing;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Player
    {
        public WebSocket Socket { get; }
        public Snek Snake { get; }

        public Player(WebSocket socket, Arena arena, Point spawnPoint)
        {
            Socket = socket;
            Snake = new Snek(arena, spawnPoint);

            Task.Run(HandleDirectionChanges);
        }

        private async Task HandleDirectionChanges()
        {
            var buf = new byte[1];
            while (true)
            {
                await Socket.ReceiveAsync(new System.ArraySegment<byte>(buf), CancellationToken.None);
                Snake.ChangeDirection((Direction)buf[0]);
            }
        }

        public Player(WebSocket socket, Arena arena)
            : this(socket, arena, new Point(1, 1))
        {
        }
    }
}