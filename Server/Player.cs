using System.Drawing;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Player
    {
        public WebSocket Socket { get; }
        
        // TrySet anything to this object to disconnect the player. Frees the websocket.
        public TaskCompletionSource<object> PlayerDisconnected { get; }
        public Snek Snake { get; }

        private Task _previousMessage;
        private int _deliveryFailCount;
        private const int MaxDeliveryAttempts = 5;

        public Player(WebSocket socket, TaskCompletionSource<object> playerDisconnected, Arena arena, Point spawnPoint, Color color)
        {
            Socket = socket;
            PlayerDisconnected = playerDisconnected;
            Snake = new Snek(arena, spawnPoint, color);

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

        public Player(WebSocket socket, TaskCompletionSource<object> playerDisconnected, Arena arena, Color color)
            : this(socket, playerDisconnected, arena, new Point(1, 1), color)
        {
        }

        public bool CheckDisconnected(Task task)
        {
            // Check if a message is set.
            if (_previousMessage == null)
            {
                _previousMessage = task;
                return false;
            }

            // Check if the previously set message eventually completed.
            if (_previousMessage.IsCompletedSuccessfully)
            {
                _previousMessage = null;
                _deliveryFailCount = 0;
                return false;
            }

            return _deliveryFailCount++ > MaxDeliveryAttempts;
        }
    }
}