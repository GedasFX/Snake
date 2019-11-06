using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Server.ArenaItems;
using Server.Facades;

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

                        var message = new Message { Arena = arena.ColorMap };
                        foreach (var p in arena.Players)
                        {
                            p.OnNext(message);
                        }

                        // Generate food.
                        arena.FoodSpawningFacade.ExecuteTick();

                        // Wait until next server tick.
                        // Logger.Instance.LogMessage("Waiting until next tick ...");

                        // Run the game slower
                        await Task.Delay(250);
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