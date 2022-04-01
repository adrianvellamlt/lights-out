using System;
using System.Text.RegularExpressions;
using HashidsNet;
using LightsOut.GameLogic;
using LightsOut.Infrastructure;
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
            services.AddTransient<ISystemClock, RealSystemClock>();

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

                endpoints.MapGet("/play", async context =>
                {
                    var gameStateService = context.RequestServices.GetRequiredService<IGameStateService>();

                    var gameVisualizer = context.RequestServices.GetRequiredService<IGameVisualizer>();

                    var gameState = await gameStateService.InitializeGameAsync(context.RequestAborted);

                    context.Response.Headers.Add("X-GameId", gameState.Id.ToString());

                    context.Response.Headers.ContentType = "text/plain; charset=UTF-8";

                    await context.Response.WriteAsync(gameVisualizer.Draw(gameState.Game), context.RequestAborted);
                });

                endpoints.MapGet("/draw/{gameId}", async context => 
                {
                    var gameStateService = context.RequestServices.GetRequiredService<IGameStateService>();

                    var gameVisualizer = context.RequestServices.GetRequiredService<IGameVisualizer>();

                    var routeData = context.GetRouteData();

                    if (
                        !routeData.Values.TryGetValue("gameId", out var routeId) || 
                        routeId is not string gameHash ||
                        !Guid.TryParse(gameHash, out var gameId)
                    )
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;

                        return;
                    }

                    var gameState = await gameStateService.GetLastStateAsync(gameId, context.RequestAborted);

                    if (gameState == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;

                        return;
                    }

                    context.Response.Headers.ContentType = "text/plain; charset=UTF-8";
                    
                    if (gameState.SurrenderedAtUTC.HasValue)
                    {
                        await context.Response.WriteAsync("Game Over! You took too long to solve this puzzle.", context.RequestAborted);
                    }
                    else
                    {
                        await context.Response.WriteAsync(gameVisualizer.Draw(gameState.Game), context.RequestAborted);
                    }
                });

                endpoints.MapGet("/toggle/{gameId}/{cell}", async context =>
                {
                    var gameStateService = context.RequestServices.GetRequiredService<IGameStateService>();

                    var gameVisualizer = context.RequestServices.GetRequiredService<IGameVisualizer>();
                    
                    var routeData = context.GetRouteData();

                    var regex = new Regex("([rR][0-9]+)|([cC][0-9]+)");

                    if (
                        !routeData.Values.TryGetValue("gameId", out var routeId) || 
                        routeId is not string gameHash ||
                        !Guid.TryParse(gameHash, out var gameId) ||

                        !routeData.Values.TryGetValue("cell", out var routeCell) || 
                        routeCell is not string cell ||
                        !regex.IsMatch(cell)
                    )
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;

                        return;
                    }
                    
                    var matches = regex.Matches(cell);

                    var gameState = await gameStateService.GetLastStateAsync(gameId, context.RequestAborted);

                    if (gameState == null || matches.Count != 2)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;

                        return;
                    }

                    byte row, column;

                    if (matches[0].Value.StartsWith('r'))
                    {
                        row = byte.Parse(matches[0].Value[1..]);
                        column = byte.Parse(matches[1].Value[1..]);
                    }
                    else
                    {
                        row = byte.Parse(matches[1].Value[1..]);
                        column = byte.Parse(matches[0].Value[1..]);
                    }

                    gameState.Game.ToggleCell(row, column);

                    context.Response.Headers.ContentType = "text/plain; charset=UTF-8";

                    await context.Response.WriteAsync(gameVisualizer.Draw(gameState.Game), context.RequestAborted);
                });

                endpoints.MapRazorPages();
            });
        }
    }
}