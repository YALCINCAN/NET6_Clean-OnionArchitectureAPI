using Application.Interfaces.Services;
using Common.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

public class RedisCacheService : ICacheService
{
    private readonly CacheSettings _cacheSettings;
    private readonly ConnectionMultiplexer _redisConnection;
    private readonly IDatabase _database;
    private readonly IServer _server;

    public RedisCacheService(IOptions<CacheSettings> cacheSettings)
    {
        _redisConnection = ConnectionMultiplexer.Connect(cacheSettings.Value.RedisURL);
        _database = GetDatabase();
        _server = GetServer();
    }

    private IDatabase GetDatabase()
    {
        return _redisConnection.GetDatabase();
    }

    private IServer GetServer()
    {
        return _redisConnection.GetServer(_redisConnection.GetEndPoints().First());
    }

    public T Get<T>(string key)
    {
        var value = _database.StringGet(key);
        return value.IsNull ? default : Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken token = default)
    {
        var value = await _database.StringGetAsync(key);
        return value.IsNull ? default : Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
    }

    public void Refresh(string key)
    {
        _database.KeyExpire(key, TimeSpan.FromMinutes(30));
    }

    public async Task RefreshAsync(string key, CancellationToken token = default)
    {
        await _database.KeyExpireAsync(key, TimeSpan.FromMinutes(30));
    }

    public void Remove(string key)
    {
        _database.KeyDelete(key);
    }

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        await _database.KeyDeleteAsync(key);
    }

    public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null)
    {
        var jsonValue = Newtonsoft.Json.JsonConvert.SerializeObject(value);

        if (slidingExpiration.HasValue)
        {
            _database.StringSet(key, jsonValue, slidingExpiration);
        }
        else
        {
            _database.StringSet(key, jsonValue);
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
    {
        var jsonValue = Newtonsoft.Json.JsonConvert.SerializeObject(value);

        if (slidingExpiration.HasValue)
        {
            await _database.StringSetAsync(key, jsonValue, slidingExpiration);
        }
        else
        {
            await _database.StringSetAsync(key, jsonValue);
        }
    }

    public void RemoveByPrefix(string prefix)
    {
        var keys = _server.Keys(pattern: $"{prefix}*");

        foreach (var key in keys)
        {
            _database.KeyDelete(key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix)
    {
        var keys = _server.Keys(pattern: $"{prefix}*");

        foreach (var key in keys)
        {
            await _database.KeyDeleteAsync(key);
        }
    }

    public void ClearAll()
    {
        var keys = _server.Keys().ToList();

        foreach (var key in keys)
        {
            _database.KeyDelete(key);
        }
    }

    public async Task ClearAllAsync()
    {
        var keys = _server.Keys().ToList();

        foreach (var key in keys)
        {
            await _database.KeyDeleteAsync(key);
        }
    }


}
