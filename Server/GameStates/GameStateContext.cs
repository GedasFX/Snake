using System;

namespace Server.GameStates
{
    /// <summary>
    /// Context for game state objects.
    /// </summary>
    public class GameStateContext
    {
        // Holds the current state of the game
        private IGameState _currentState;

        /// <summary>
        /// Constructs the game state context and sets some default values for the state durations.
        /// </summary>
        /// <param name="arena">Arena where the game will take place</param>
        public GameStateContext(Arena arena)
        {
            // Set some default values for state durations
            PregameCountdownStateDuration = 50;
            GameInProgressStateDuration = 150;
            GameEndingSoonStateDuration = 50;
            PostGameCountdownStateDuration = 100;

            _currentState = new WaitingForPlayersToConnectState(arena, this);
            _currentState.OnStateEnter();
        }

        /// <summary>
        /// Runs a tick of the current state.
        /// </summary>
        public void Run() => _currentState.RunTick();

        /// <summary>
        /// Changes the state of the context.
        /// </summary>
        /// <param name="newState">State to change the context to</param>
        public void ChangeState(IGameState newState)
        {
            _currentState = newState;
            _currentState.OnStateEnter();
        }

        /// <summary>
        /// Gets the current state of the game as a value of a GameStatesEnum.
        /// </summary>
        public GameStateEnum GetStateOfGameAsEnum() => _currentState.ToEnum();

        #region State durations

        private int _pregameCountdownStateDuration;

        /// <summary>
        /// Duration of the pregame countdown state.
        /// </summary>
        public int PregameCountdownStateDuration
        {
            get => _pregameCountdownStateDuration;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        "Pregame countdown state duration can't be negative!");

                _pregameCountdownStateDuration = value;
            }
        }

        private int _gameInProgressStateDuration;

        /// <summary>
        /// Duration of the game in progress state.
        /// </summary>
        public int GameInProgressStateDuration
        {
            get => _gameInProgressStateDuration;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        "Game in progress state duration can't be negative!");

                _gameInProgressStateDuration = value;
            }
        }

        private int _gameEndingSoonStateDuration;

        /// <summary>
        /// Duration of the game ending soon state.
        /// </summary>
        public int GameEndingSoonStateDuration
        {
            get => _gameEndingSoonStateDuration;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        "Game ending soon countdown state duration can't be negative!");

                _gameEndingSoonStateDuration = value;
            }
        }

        private int _postGameCountdownStateDuration;

        /// <summary>
        /// Duration of the post-game countdown state.
        /// </summary>
        public int PostGameCountdownStateDuration
        {
            get => _postGameCountdownStateDuration;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        "Post-game countdown state duration can't be negative!");

                _postGameCountdownStateDuration = value;
            }
        }

        #endregion
    }
}