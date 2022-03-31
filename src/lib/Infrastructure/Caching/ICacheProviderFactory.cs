using System;
using Microsoft.Extensions.DependencyInjection;

namespace LightsOut.Infrastructure
{
    public interface ICacheProviderFactory
    {
        ICache<T> GetCacheInstance<T>(CacheType cacheType);
    }

    public class CacheProviderFactory : ICacheProviderFactory
    {
        private IServiceProvider ServiceProvider { get; }

        public CacheProviderFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public ICache<T> GetCacheInstance<T>(CacheType cacheType) => cacheType switch
        {
            CacheType.InMemory => (ICache<T>)ServiceProvider.GetRequiredService(typeof(InMemoryCache<T>)),
            _ => throw new NotImplementedException("Cache type has not been implemented yet."),
        };
    }
}