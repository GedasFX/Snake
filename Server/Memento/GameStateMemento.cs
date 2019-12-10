using System;
using Server.GameStates;

namespace Server.Memento
{
    /// <summary>
    /// A memento object which stores a game state.
    /// </summary>
    public class GameStateMemento
    {
        /// <summary>
        /// The game state stored in this memento.
        /// </summary>
        public IGameState StoredGameState { get; }

        /// <summary>
        /// Constructs a game state memento.
        /// </summary>
        /// <param name="state">Game state to store.</param>
        public GameStateMemento(IGameState state)
        {
            StoredGameState =
                state ?? throw new ArgumentNullException(nameof(state),
                    "Null passed to game state memento constructor!");
        }
    }
}
