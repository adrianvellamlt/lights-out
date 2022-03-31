using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            services.AddRazorPages();

            services.AddScoped<IGameVisualizer, TextGameVisualizer>();
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

                endpoints.MapGet("/draw", async context => 
                {
                    var gameState = new GameLogic.LightsOut(5, 5, 10); //temporarily initializing a game each time

                    var gameVisualizer = context.RequestServices.GetRequiredService<IGameVisualizer>();

                    context.Response.Headers.ContentType = "text/plain; charset=UTF-8";

                    await context.Response.WriteAsync(gameVisualizer.Draw(gameState), context.RequestAborted);
                });

                endpoints.MapRazorPages();
            });
        }
    }
}