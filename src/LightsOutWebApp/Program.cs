using Serilog;
using Microsoft.AspNetCore.Builder;

namespace LightsOut.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((ctx, loggerConf) =>
            {
                loggerConf
                    .ReadFrom.Configuration(ctx.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName);
            });
                
            var startup = new Startup(builder.Configuration);

            startup.ConfigureServices(builder.Services);

            var app = builder.Build();

            startup.Configure(app, app.Environment);

            app.Run();
        }
    }
}
