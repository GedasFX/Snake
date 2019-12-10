namespace Server.Memento
{
    /// <summary>
    /// Interface for the originator (the game state context)
    /// </summary>
    public interface IGameStateOriginator
    {
        /// <summary>
        /// Constructs a game state memento from the current state of the originator and returns it.
        /// </summary>
        /// <returns>The created memento.</returns>
        GameStateMemento CreateGameStateMemento();

        /// <summary>
        /// Restores the state of the originator from the given memento object.
        /// </summary>
        /// <param name="memento">Memento to restore the state from.</param>
        void RestoreFromGameStateMemento(GameStateMemento memento);
    }
}
