using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LightsOut.Web
{
    public class BootstrapperService : BackgroundService
    {
        private ILogger<BootstrapperService> Logger { get; }
        private IServiceProvider ServiceProvider { get; }

        public BootstrapperService(ILogger<BootstrapperService> logger, IServiceProvider serviceProvider)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting Bootstrapper service");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Done bootsrapping!");

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // no need to fire them all at once, so I'm going to do it sequentially for now.
            foreach (var bootstrapper in ServiceProvider.GetServices<IBootstrapper>())
            {
                await bootstrapper.BootstrapAsync(stoppingToken);
            }

            await StopAsync(stoppingToken);
        }
    }
}