using Microsoft.Extensions.Caching.Memory;

namespace BlazeGate.SingleFlightMemoryCache
{
    public sealed class SingleFlightMemoryCache : ISingleFlightMemoryCache
    {
        private readonly IMemoryCache cache;
        private readonly IKeyedLock keyedLock;

        public SingleFlightMemoryCache(IMemoryCache cache, IKeyedLock keyedLock)
        {
            this.cache = cache;
            this.keyedLock = keyedLock;
        }

        public Task<TItem?> GetOrCreateAsync<TItem>(string key, Func<ICacheEntry, Task<TItem?>> factory)
            => GetOrCreateAsync(key, factory, CancellationToken.None);

        public async Task<TItem?> GetOrCreateAsync<TItem>(string key, Func<ICacheEntry, Task<TItem?>> factory, CancellationToken cancellationToken)
        {
            if (cache.TryGetValue(key, out TItem? existing))
            {
                return existing;
            }

            // 这里用独立锁 key，避免出现 object key 的 Equals/ToString 不确定导致冲突
            var lockKey = $"IMemoryCache_SingleFlight_{key}";

            using (await keyedLock.AcquireAsync(lockKey, cancellationToken))
            {
                if (cache.TryGetValue(key, out existing))
                {
                    return existing;
                }

                using ICacheEntry entry = cache.CreateEntry(key);

                var result = await factory(entry).ConfigureAwait(false);
                entry.Value = result;

                return result;
            }
        }
    }

    public interface ISingleFlightMemoryCache
    {
        Task<TItem?> GetOrCreateAsync<TItem>(string key, Func<ICacheEntry, Task<TItem?>> factory);

        Task<TItem?> GetOrCreateAsync<TItem>(string key, Func<ICacheEntry, Task<TItem?>> factory, CancellationToken cancellationToken);
    }
}