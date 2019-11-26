using System;
using System.Linq;

namespace Server.GameStates
{
    public class WaitingForPlayersToConnectState : IGameState
    {
        private readonly Arena _arena;
        private readonly GameStateContext _context;

        public WaitingForPlayersToConnectState(Arena arena, GameStateContext context)
        {
            _arena = arena;
            _context = context;
        }

        public void OnStateEnter()
        {
            const string message = "[GAME STATE] No players connected: waiting for at least one to join.";
            Logger.Instance.LogWithColor(ConsoleColor.Blue, message);
            _arena.Reset();
        }

        public void RunTick()
        {
            // Switch to pregame state if there are players connected, otherwise wait.
            if (!_arena.Players.Any())
                return;

            var pregameCountdownState = new PregameCountdownState(_arena, _context);
            _context.ChangeState(pregameCountdownState);
        }

        public GameStateEnum ToEnum() => GameStateEnum.Pending;
    }
}
