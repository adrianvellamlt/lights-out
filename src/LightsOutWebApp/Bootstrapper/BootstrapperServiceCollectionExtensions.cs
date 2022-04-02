using LightsOut.Web;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BootstrapperServiceCollectionExtensions
    {
        public static IServiceCollection AddBootrappers(this IServiceCollection services)
        {
            services.AddSingleton<IBootstrapper, DatabaseMigratorBootstrapper>();

            services.AddSingleton<BootstrapperService>();

            return services;
        }
    }
}