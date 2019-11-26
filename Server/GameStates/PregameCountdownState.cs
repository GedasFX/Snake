using System;
using System.Linq;

namespace Server.GameStates
{
    /// <summary>
    /// Represents the state of the pregame countdown.
    /// </summary>
    public class PregameCountdownState : IGameState
    {
        private readonly Arena _arena;
        private readonly GameStateContext _context;
        private int _ticksLeftUntilGameStarts;

        /// <summary>
        /// Constructs the pregame countdown state object.
        /// </summary>
        /// <param name="arena">Arena where the game takes place</param>
        /// <param name="context">Game state context</param>
        public PregameCountdownState(Arena arena, GameStateContext context)
        {
            _arena = arena;
            _context = context;
            _ticksLeftUntilGameStarts = _context.PregameCountdownStateDuration;
        }

        public void OnStateEnter()
        {
            Logger.Instance.LogWithColor(ConsoleColor.Blue, "[GAME STATE] Pregame countdown has started!");
            _arena.Reset();
        }

        public void RunTick()
        {
            if (_ticksLeftUntilGameStarts > 0)
            {
                if(_ticksLeftUntilGameStarts % 10 == 0)
                    Logger.Instance.LogWithColor(ConsoleColor.Blue,
                        $"[GAME STATE] {_ticksLeftUntilGameStarts} ticks left until the game starts!");

                _ticksLeftUntilGameStarts--;

                // If no players are connected during the countdown, wait for at least one to join, then restart.
                if (!_arena.Players.Any())
                {
                    var waitState = new WaitingForPlayersToConnectState(_arena, _context);
                    _context.ChangeState(waitState);
                }
            }
            else
            {
                // Countdown elapsed, game has started.
                var gameInProgressState = new GameInProgressState(_arena, _context);
                _context.ChangeState(gameInProgressState);
            }
        }

        public GameStateEnum ToEnum() => GameStateEnum.Pregame;
    }
}