using System;
using System.Linq;
using System.Threading.Tasks;
using Server.GameStates;

namespace Server
{
    public class ArenaAdapter : Arena
    {
        private readonly ArenaAdaptee _adaptee = new ArenaAdaptee();

        public override Task StartAsync() => base.StartAsync();

        private class ArenaAdaptee
        {
            public async Task StartAsync(Arena arena)
            {
                while (true)
                {
                    try
                    {
                        Message message;
                        var currentStateOfGame = arena.GameStateContext.GetStateOfGameAsEnum();
                        if(currentStateOfGame != GameStateEnum.PostGame)
                            message = new Message(arena.ColorMap, currentStateOfGame, null);
                        else
                        {
                            // Only send podium data when the game has finished.
                            var podium = arena.GetPlayerStandings().Take(3).ToArray();
                            message = new Message(arena.ColorMap, currentStateOfGame, podium);
                        }

                        foreach (var p in arena.Players)
                            p.OnNext(message);

                        // Run game
                        arena.GameStateContext.Run();

                        // Run the game slower
                        await Task.Delay(100);
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.LogError(e.StackTrace);
                    }
                }
            }
        }
    }
}