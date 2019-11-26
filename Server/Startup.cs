using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Arena, ArenaAdapter>();
        }



        private class C : IEnumerable<int>
        {
            public IEnumerator<int> GetEnumerator() => B().GetEnumerator();

            private IEnumerable<int> B()
            {
                var rand = new Random();
                int radn;
                do
                {
                    radn = rand.Next(200);
                    yield return radn;
                } while (radn < 150);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Arena arena)
        {


            Task.Run(() =>
            {
                var c = new C();
                foreach (var item in c)
                {
                    Console.WriteLine(item);
                }
            }).Wait();

            _ = arena.StartAsync();

            app.UseWebSockets();
            app.Run(async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    var task = new TaskCompletionSource<object>();
                    arena.Players.Connect(webSocket, task);
                    await task.Task;
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            });
        }
    }
}
