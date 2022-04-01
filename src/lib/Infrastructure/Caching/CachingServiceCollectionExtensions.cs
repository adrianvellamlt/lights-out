using LightsOut.Infrastructure;
using Microsoft.Extensions.Caching.Memory;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CachingServiceCollectionExtensions
    {
        public static IServiceCollection AddCaching(this IServiceCollection services)
        {
            services.AddSingleton<IMemoryCache, MemoryCache>();

            services.AddSingleton(typeof(InMemoryCache<>));

            services.AddScoped<ICacheProviderFactory, CacheProviderFactory>();

            return services;
        }
    }
}