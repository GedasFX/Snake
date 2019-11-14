using System;
using System.Drawing;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server
{
    public class Player : IObserver<Message>, IDisposable
    {
        private WebSocket Socket { get; }

        // TrySet anything to this object to disconnect the player. Frees the websocket.
        private TaskCompletionSource<object> PlayerDisconnected { get; }
        private Arena Arena { get; }
        private Snek Snake { get; }

        private Task _previousMessage;
        private int _deliveryFailCount;
        private const int MaxDeliveryAttempts = 5;

        private Player(WebSocket socket, TaskCompletionSource<object> playerDisconnected, Arena arena, Point spawnPoint, Color color)
        {
            Socket = socket;
            PlayerDisconnected = playerDisconnected;
            Arena = arena;
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
            : this(socket, playerDisconnected, arena, arena.GetSpawnPoint(), color)
        {
        }

        private bool CheckDisconnected(Task task)
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

        public void OnCompleted()
        {
            // Game never ends.
        }

        public void OnError(Exception error)
        {
            Logger.Instance.LogError(error.Message);
        }

        public void OnNext(Message message)
        {
            // Serialize into JSON
            var ser = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message));

            // Send the message
            using var ct = new CancellationTokenSource();
            ct.CancelAfter(250);

            var task = Socket.SendAsync(new ArraySegment<byte>(ser), WebSocketMessageType.Text,
                true, ct.Token);
            if (CheckDisconnected(task))
            {
                Dispose();
                return;
            }

            Snake.Move();
        }

        /// <summary>
        /// Resets the snake by shrinking it back to its initial starting size and moving it to
        /// a random suitable location in the arena. It's as if the player just recently joined the game.
        /// Called at the beginning of each game.
        /// </summary>
        public void ResetSnake() => Snake.Reset();

        public void Dispose()
        {
            Arena.Players.Remove(this);
            foreach (var point in Snake.Body)
            {
                Arena.UpdateCell(point.X, point.Y, null);
            }
            PlayerDisconnected.TrySetResult(null);
        }
    }
}