using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace HomeBudget.Cache
{
    

    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly CacheSettings _cacheSettings;
        private MemoryCacheEntryOptions _cacheOptions;

        public MemoryCacheService(IMemoryCache memoryCache, IOptions<CacheSettings> cacheSettings)
        {
            _memoryCache = memoryCache;
            _cacheSettings = cacheSettings.Value;

            if(_cacheSettings != null)
            {
                _cacheOptions = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(1),
                    AbsoluteExpiration = DateTime.Now.AddMinutes(_cacheSettings.AbsoluteExpirationInMinutes),
                    Priority = CacheItemPriority.Normal,
                    Size = 512
                };
            }
        }
                    
        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }

        public T Set<T>(string cacheKey, T value)
        {
            return _memoryCache.Set(cacheKey, value, _cacheOptions);
        }

        public bool TryGet<T>(string cacheKey, out T value)
        {
            _memoryCache.TryGetValue(cacheKey, out value);
            if (value == null) return false;
            else return true;
        }
    }
}
                    
                    
                    
