namespace HomeBudget.Cache
{
    public class RedisCacheService : ICacheService
    {
        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public T Set<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public bool TryGet<T>(string key, out T value)
        {
            throw new NotImplementedException();
        }
    }
}
