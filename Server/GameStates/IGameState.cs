namespace Server.GameStates
{
    /// <summary>
    /// Interface for a concrete state of the game.
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// Code specific to the state that is run after the context changes to the aforementioned state.
        /// </summary>
        void OnStateEnter();

        /// <summary>
        /// Code specific to the state that is run every tick.
        /// </summary>
        void RunTick();

        /// <summary>
        /// Gets the corresponding GameStateEnum value for the concrete state.
        /// </summary>
        GameStateEnum ToEnum();
    }
}
