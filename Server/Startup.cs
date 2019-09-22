using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Arena>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Arena arena)
        {
            arena.StartAsync().ConfigureAwait(false);

            app.UseWebSockets();
            app.Run(async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using (var webSocket = await context.WebSockets.AcceptWebSocketAsync())
                    {
                        var task = new TaskCompletionSource<object>();
                        arena.AddConnection(webSocket, task);
                        await task.Task;
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            });
        }
    }
}
