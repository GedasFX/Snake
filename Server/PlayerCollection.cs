using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Server
{
    public class PlayerCollection : IObservable<Message>, IEnumerable<Player>
    {
        private List<Player> Players { get; } = new List<Player>();
        private Arena Arena { get; }

        public PlayerCollection(Arena arena)
        {
            Arena = arena;
        }

        public void Connect(WebSocket socket, TaskCompletionSource<object> playerDisconnected)
        {
            var random = new Random();
            Subscribe(new Player(socket, playerDisconnected, Arena, this,
                Color.FromArgb(random.Next(255), random.Next(255), random.Next(255))));
        }

        public void Disconnect(Player player)
        {
            Players.Remove(player);
        }

        public IEnumerator<Player> GetEnumerator()
        {
            return new PlayerEnumerator(Players);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void BroadcastMessage(string message)
        {
            UpdateAll(new Message(message));
        }

        public void UpdateAll(Message message)
        {
            using var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current?.OnNext(message);
            }
        }

        public IDisposable Subscribe(IObserver<Message> observer)
        {
            if (!(observer is Player player))
                throw new ArgumentException("Observer must be of type Player", nameof(observer));

            Players.Add(player);

            return player;
        }
    }

    internal class PlayerEnumerator : IEnumerator<Player>
    {
        private int _position = -1;
        private readonly IList<Player> _players;

        public Player Current => _players[_position];

        public PlayerEnumerator(IList<Player> players)
        {
            _players = players;
        }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            return ++_position < _players.Count;
        }

        public void Reset()
        {
            _position = -1;
        }

        public void Dispose() { }
    }
}
