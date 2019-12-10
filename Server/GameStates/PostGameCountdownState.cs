using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Server.GameStates
{
    /// <summary>
    /// Represents the state of post-game.
    /// </summary>
    public class PostGameCountdownState : IGameState
    {
        private readonly Arena _arena;
        private readonly GameStateContext _context;
        private int _ticksLeftUntilPregameStart;

        /// <summary>
        /// Constructs a state object representing the state of post-game.
        /// </summary>
        /// <param name="arena"></param>
        /// <param name="context"></param>
        public PostGameCountdownState(Arena arena, GameStateContext context)
        {
            _arena = arena;
            _context = context;
            _ticksLeftUntilPregameStart = _context.PostGameCountdownStateDuration;
        }

        // Log player standings, clear context's caretaker.
        public void OnStateEnter()
        {
            Logger.Instance.LogWithColor(ConsoleColor.Blue, "[GAME STATE] Game has ended!");

            // The standings are a bit crap because the colors aren't particularly human readable, but whatever.
            var playerStandings = _arena.GetPlayerStandings();
            
            Logger.Instance.LogWithColor(ConsoleColor.Green, "====    PLAYER STANDINGS    ====");
            Logger.Instance.LogWithColor(ConsoleColor.Green, "==== Sorted by snake length ====");

            foreach (var element in playerStandings)
            {
                string line = $"Player (color: {element.PlayerColor.ToString()}): {element.SnakeLength}";
                Logger.Instance.LogWithColor(ConsoleColor.Green, line);
            }
            
            // Remove previously held memento in caretaker.
            _context.Caretaker.RemoveStoredMemento();
        }

        public void RunTick()
        {
            if (_ticksLeftUntilPregameStart > 0)
            {
                if(_ticksLeftUntilPregameStart % 10 == 0)
                {
                    var message =
                        $"[GAME STATE] If any players are left, the pregame countdown will start in {_ticksLeftUntilPregameStart} ticks.";
                    Logger.Instance.LogWithColor(ConsoleColor.Blue, message);
                }

                _ticksLeftUntilPregameStart--;

                // If no players are connected, let the state run its course.
            }
            else
            {
                // Post-game ended, go to pregame.
                var pregameCountdownState = new PregameCountdownState(_arena, _context);
                _context.ChangeState(pregameCountdownState);
            }
        }

        public GameStateEnum ToEnum() => GameStateEnum.PostGame;
        public int GetRemainingDuration() => _ticksLeftUntilPregameStart;
    }
}