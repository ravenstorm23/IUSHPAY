using StackExchange.Redis;
using System.Text.Json;
using IUSHPAY.Application.Common.Interfaces;

namespace IUSHPAY.Infrastructure.Cache;

public class RedisCacheService : ICacheService
{
	private readonly IDatabase _db;

	public RedisCacheService(IConnectionMultiplexer redis)
	{
		_db = redis.GetDatabase();
	}

	public async Task<T?> GetAsync<T>(string key)
	{
		var value = await _db.StringGetAsync(key);
		return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value!);
	}

	public Task SetAsync<T>(string key, T value, TimeSpan expiry)
	{
		return _db.StringSetAsync(key, JsonSerializer.Serialize(value), expiry);
	}

	public Task RemoveAsync(string key)
	{
		return _db.KeyDeleteAsync(key);
	}
}
