using System;

namespace Server.GameStates
{
    /// <summary>
    /// Represents the state of the game being in progress and ending soon.
    /// </summary>
    public class GameEndingSoonState : IGameState
    {
        private readonly Arena _arena;
        private readonly GameStateContext _context;
        private int _ticksLeft;

        /// <summary>
        /// Constructs a state object, representing the state of the game being in progress and ending soon.
        /// </summary>
        /// <param name="arena">Arena where the game takes place</param>
        /// <param name="context">Game state context</param>
        public GameEndingSoonState(Arena arena, GameStateContext context)
        {
            _arena = arena;
            _context = context;
            _ticksLeft = _context.GameEndingSoonStateDuration;
        }

        // Log state change to console.
        public void OnStateEnter() =>
            Logger.Instance.LogWithColor(ConsoleColor.Blue, "[GAME STATE] Game will end soon!");

        // Runs a tick of the food spawning facade, while doing some occasional logging to the console.
        public void RunTick()
        {
            if (_ticksLeft > 0)
            {
                if (_ticksLeft % 10 == 0)
                {
                    string message = $"[GAME STATE] Game will end in {_ticksLeft} ticks.";
                    Logger.Instance.LogWithColor(ConsoleColor.Blue, message);
                }

                _arena.FoodSpawningFacade.ExecuteTick();
                _ticksLeft--;
            }
            else
            {
                // Game has ended, go to post-game.
                var postGameCountdownState = new PostGameCountdownState(_arena, _context);
                _context.ChangeState(postGameCountdownState);
            }
        }

        public GameStateEnum ToEnum() => GameStateEnum.EndingSoon;
    }
}