using System;
using System.IO;
using System.Reflection;
using FluentMigrator.Runner;
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
using Microsoft.OpenApi.Models;

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
            var sqliteConn = Configuration.GetConnectionString("SQLite");

            // register migrations
            services
                .AddFluentMigratorCore()
                .ConfigureRunner(options => options
                    .AddSQLite()
                    .WithGlobalConnectionString(sqliteConn)
                    .ScanIn(typeof(GameLogic.LightsOut).Assembly).For.Migrations()
                );

            services.AddOptions();

            services.Configure<ConnectionStringOptions>(Configuration.GetSection("ConnectionStrings"));

            // one of the bootstrappers is the db migrator
            services.AddBootrappers();

            services.AddLogging(options =>
            {
                options.AddFluentMigratorConsole();
            });

            services.AddTransient<ISystemClock, RealSystemClock>();

            services.AddCaching();

            //TODO:" not using it currently. maybe remove later
            services.AddSingleton<IHashids>(_ => new Hashids(Configuration.GetValue<string>("Hashids:Salt")));
            
            services.AddLightsOut();

            services.AddScoped<IGameVisualizer, HtmlGameVisualizer>();

            services.AddControllers().AddNewtonsoftJson();

            services.AddRazorPages();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "LightOut API",
                    Description = "Provides functionalities to the lightsout game"
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            services.AddHostedService<BootstrapperService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                endpoints.MapControllers();

                endpoints.MapGet("/ping", context => context.Response.WriteAsync("pong", context.RequestAborted));
            });
        }
    }
}