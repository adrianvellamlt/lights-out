using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightsOut.Infrastructure
{
    public interface ICache<T>
    {
        Task<bool> ContainsKey(string key);

        Task<T?> GetAsync(string key);

        Task<T?> GetOrCreateAsync(string key, Func<Task<T?>> getFunc, TimeSpan expiration, TimeSpan? slidingExpiration = null, CancellationTokenSource? cts = null);

        Task SetAsync(string key, T item, TimeSpan expiration, TimeSpan? slidingExpiration = null, CancellationTokenSource? cts = null);

        Task RemoveAsync(string key);
    }
}