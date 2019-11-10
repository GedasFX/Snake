using System;
using System.Threading.Tasks;

namespace Server
{
    public class ArenaAdapter : Arena
    {
        private readonly ArenaAdaptee _adaptee = new ArenaAdaptee();

        public override Task StartAsync() => _adaptee.StartAsync(this);

        private class ArenaAdaptee
        {
            public async Task StartAsync(Arena arena)
            {
                while (true)
                {
                    try
                    {
                        // Print the number of ticks elapsed.
                        // Logger.Instance.LogMessage($"Number of ticks elapsed: {cycle++}");

                        // Update every player
                        // Logger.Instance.LogMessage($"Updating {Players.Count} player(s)");

                        var message = new Message(arena.ColorMap);
                        foreach (var p in arena.Players)
                        {
                            p.OnNext(message);
                        }

                        // Generate food.
                        arena.FoodSpawningFacade.ExecuteTick();

                        // Wait until next server tick.
                        // Logger.Instance.LogMessage("Waiting until next tick ...");

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