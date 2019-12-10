using System;

namespace Server.Memento
{
    /// <summary>
    /// Stores a single game state memento.
    /// </summary>
    public class GameStateCaretaker
    {
        private GameStateMemento memento;

        public GameStateMemento Memento
        {
            get => memento;
            set => memento = value ?? throw new ArgumentNullException(nameof(value), "Null memento passed to caretaker!");
        }

        /// <summary>
        /// Removes the currently stored memento in the caretaker.
        /// </summary>
        public void RemoveStoredMemento()
        {
            if (memento == null)
                return;

            Logger.Instance.LogWithColor(ConsoleColor.Magenta, "[MEMENTO] Removing previously stored memento in caretaker.");
            memento = null;
        }
    }
}