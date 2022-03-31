using HashidsNet;
using LightsOut.GameLogic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LightsOut.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCaching();

            services.AddSingleton<IHashids>(_ => new Hashids(Configuration.GetValue<string>("Hashids:Salt")));
            
            services.AddLightsOut();

            services.AddScoped<IGameVisualizer, TextGameVisualizer>();

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/ping", async context => 
                {
                    await context.Response.WriteAsync("pong", context.RequestAborted);
                });

                endpoints.MapGet("/draw/{gameId}", async context => 
                {
                    var gameStateService = context.RequestServices.GetRequiredService<IGameStateService>();

                    var gameVisualizer = context.RequestServices.GetRequiredService<IGameVisualizer>();

                    var hashIds = context.RequestServices.GetRequiredService<IHashids>();

                    var routeData = context.GetRouteData();

                    if (!routeData.Values.TryGetValue("gameId", out var routeId) || routeId is not string gameHash)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;

                        return;
                    }

                    var decoded = hashIds.Decode(gameHash);
                    
                    if (decoded.Length != 1)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;

                        await context.Response.WriteAsync("Malformed game code");

                        return;
                    }

                    var gameState = await gameStateService.GetLastStateAsync(decoded[0], context.RequestAborted);

                    if (gameState == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;

                        return;
                    }

                    context.Response.Headers.ContentType = "text/plain; charset=UTF-8";
                    
                    if (gameState.SurrenderedAtUtc.HasValue)
                    {
                        await context.Response.WriteAsync("Game Over! You took too long to solve this puzzle.", context.RequestAborted);
                    }
                    else
                    {
                        await context.Response.WriteAsync(gameVisualizer.Draw(gameState.Game), context.RequestAborted);
                    }
                });

                endpoints.MapRazorPages();
            });
        }
    }
}