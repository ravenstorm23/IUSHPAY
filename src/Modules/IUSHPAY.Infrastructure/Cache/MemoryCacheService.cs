using IUSHPAY.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace IUSHPAY.Infrastructure.Cache;

public class MemoryCacheService : ICacheService
{
	private readonly IMemoryCache _cache;

	public MemoryCacheService(IMemoryCache cache)
	{
		_cache = cache;
	}

	public Task<T?> GetAsync<T>(string key)
	{
		_cache.TryGetValue(key, out T? value);
		return Task.FromResult(value);
	}

	public Task SetAsync<T>(string key, T value, TimeSpan expiry)
	{
		_cache.Set(key, value, expiry);
		return Task.CompletedTask;
	}

	public Task RemoveAsync(string key)
	{
		_cache.Remove(key);
		return Task.CompletedTask;
	}
}