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
            
            // Reset arena only if there is no previous state.
            if(_context.Caretaker.Memento == null)
                _arena.Reset();
        }

        public void RunTick()
        {
            // Switch to pregame state if there are players connected, otherwise wait.
            if (!_arena.Players.Any())
                return;
            
            // If there is no memento in the caretaker, assume that a new game will start,
            // otherwise, restore state.
            if (_context.Caretaker.Memento == null)
            {
                var pregameCountdownState = new PregameCountdownState(_arena, _context);
                _context.ChangeState(pregameCountdownState);
            }
            else
            {
                var previousStateMemento = _context.Caretaker.Memento;
                _context.RestoreFromGameStateMemento(previousStateMemento);
            }
        }

        public GameStateEnum ToEnum() => GameStateEnum.Pending;
        
        // This state doesn't really have a set duration.
        public int GetRemainingDuration() => -1;
    }
}
