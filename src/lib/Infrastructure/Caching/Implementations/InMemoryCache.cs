using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace LightsOut.Infrastructure
{
    public class InMemoryCache<T> : ICache<T>, IDisposable
    {
        private IMemoryCache Cache { get; }

        public InMemoryCache(IMemoryCache cache) => Cache = cache;

        public Task<bool> ContainsKey(string key) => Task.FromResult(Cache.TryGetValue(key, out var _));

        public Task<T?> GetAsync(string key)
        {
            var result = Cache.Get<T>(key);

            return Task.FromResult((T?)result);
        }

        public async Task<T?> GetOrCreateAsync(string key, Func<Task<T?>> getFunc, TimeSpan expiration, TimeSpan? slidingExpiration = null, CancellationTokenSource? cts = null)
        {
            var cachedResult = await GetAsync(key);

            if (cachedResult != null) return cachedResult;

            cachedResult = await getFunc();

            if (cachedResult == null) return default;

            await SetAsync(key, cachedResult, expiration, slidingExpiration, cts);

            return cachedResult;
        }

        public Task SetAsync(string key, T item, TimeSpan expiration, TimeSpan? slidingExpiration = null, CancellationTokenSource? cts = null)
        {
            var memoryCacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            if (slidingExpiration.HasValue)
            {
                memoryCacheEntryOptions.SetSlidingExpiration(slidingExpiration.Value);
            }

            if (cts != null)
            {
                memoryCacheEntryOptions.AddExpirationToken(new Microsoft.Extensions.Primitives.CancellationChangeToken(cts.Token));
            }

            Cache.Set(key, item, memoryCacheEntryOptions);

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            Cache.Remove(key);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) => Cache.Dispose();
    }
}