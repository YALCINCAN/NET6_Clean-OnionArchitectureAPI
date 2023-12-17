using Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Reflection;

namespace Infrastructure.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }


        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }


        public Task<T> GetAsync<T>(string key, CancellationToken token = default)
        {
            return Task.FromResult(Get<T>(key));
        }

        public void Refresh(string key)
        {
            _cache.TryGetValue(key, out _);
        }


        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            Refresh(key);
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }


        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            Remove(key);
            return Task.CompletedTask;
        }

        public void RemoveByPrefix(string prefix)
        {
            var keys = GetAllKeys().Where(m => m.StartsWith(prefix));
            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {

            var keys = GetAllKeys().Where(m => m.StartsWith(prefix));
            foreach (var key in keys)
            {
                await RemoveAsync(key);
            }
        }

        public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null)
        {
            if (slidingExpiration is null)
            {
                slidingExpiration = TimeSpan.FromMinutes(10);
            }

            _cache.Set(key, value, new MemoryCacheEntryOptions { SlidingExpiration = slidingExpiration });
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken token = default)
        {
            Set(key, value, slidingExpiration);
            return Task.CompletedTask;
        }

        public void ClearAll()
        {
            var keys = GetAllKeys();
            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        public async Task ClearAllAsync()
        {
            var keys = GetAllKeys();
            foreach (var key in keys)
            {
                await RemoveAsync(key);
            }
        }

        private List<string> GetAllKeys()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            //var entries = _cache.GetType().GetField("EntriesCollection", flags).GetValue(_cache); // NET 3.1 - 5
            var entries = _cache.GetType().GetField("_entries", flags).GetValue(_cache);
            var cacheItems = entries as IDictionary;
            var keys = new List<string>();
            if (cacheItems == null) return keys;
            foreach (DictionaryEntry cacheItem in cacheItems)
            {
                keys.Add(cacheItem.Key.ToString());
            }
            return keys;
        }
    }
}
