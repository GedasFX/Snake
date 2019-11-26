using System;
using System.Linq;

namespace Server.GameStates
{
    /// <summary>
    /// Represents the state of the game being in progress and not ending soon.
    /// </summary>
    public class GameInProgressState : IGameState
    {
        private readonly Arena _arena;
        private readonly GameStateContext _context;
        private int _ticksUntilGameEndingSoonCountdown;

        /// <summary>
        /// Constructs a state object, representing the state of the game being in progress and not ending soon.
        /// </summary>
        /// <param name="arena">Arena where the game takes place</param>
        /// <param name="context">Game state context</param>
        public GameInProgressState(Arena arena, GameStateContext context)
        {
            _arena = arena;
            _context = context;
            _ticksUntilGameEndingSoonCountdown = _context.GameInProgressStateDuration;
        }

        // Log the state change to the console.
        public void OnStateEnter() => Logger.Instance.LogWithColor(ConsoleColor.Blue, "[GAME STATE] Game has started!");

        // Runs a tick of the food spawning facade, while doing some occasional logging to the console.
        public void RunTick()
        {
            if(_ticksUntilGameEndingSoonCountdown > 0)
            {
                if(_ticksUntilGameEndingSoonCountdown % 10 == 0)
                {
                    int totalGameTicksLeft = _ticksUntilGameEndingSoonCountdown + _context.GameEndingSoonStateDuration;
                    string message =
                        $"[GAME STATE] {_ticksUntilGameEndingSoonCountdown} ticks left until end of game soon countdown. Ticks left until end of game: {totalGameTicksLeft}";
                    Logger.Instance.LogWithColor(ConsoleColor.Blue, message);
                }

                _arena.FoodSpawningFacade.ExecuteTick();
                _ticksUntilGameEndingSoonCountdown--;

                // If all players disconnected, wait for at least one to join, then start from the pregame countdown again.
                if (!_arena.Players.Any())
                {
                    var waitState = new WaitingForPlayersToConnectState(_arena, _context);
                    _context.ChangeState(waitState);
                }
            }
            else
            {
                // Ending countdown has started.
                var gameEndingSoonState = new GameEndingSoonState(_arena, _context);
                _context.ChangeState(gameEndingSoonState);
            }
        }

        public GameStateEnum ToEnum() => GameStateEnum.InProgress;
    }
}