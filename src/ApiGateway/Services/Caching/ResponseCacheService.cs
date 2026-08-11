using System.Security.Cryptography;
using System.Text;
using ApiGateway.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace ApiGateway.Services.Caching;

public sealed class ResponseCacheService : IResponseCacheService
{
	private readonly IDistributedCache _cache;
	private readonly GatewayOptions _options;
	private readonly ILogger<ResponseCacheService> _logger;

	public ResponseCacheService(IDistributedCache cache, IOptions<GatewayOptions> options, ILogger<ResponseCacheService> logger)
	{
		_cache = cache;
		_options = options.Value;
		_logger = logger;
	}

	public string BuildCacheKey(string method, string? path, QueryString queryString)
	{
		using var sha256 = SHA256.Create();
		var rawKey = $"{method}:{path}:{queryString}";
		return Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(rawKey)));
	}

	public async Task<string?> TryGetAsync(string cacheKey, CancellationToken cancellationToken = default)
	{
		try
		{
			return await _cache.GetStringAsync(cacheKey, cancellationToken);
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Redis cache unavailable. Continuing without cache.");
			return null;
		}
	}

	public async Task TrySetAsync(string cacheKey, string responseBody, CancellationToken cancellationToken = default)
	{
		try
		{
			await _cache.SetStringAsync(cacheKey, responseBody, new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.CacheAbsoluteExpirationMinutes)
			}, cancellationToken);
		}
		catch
		{
			/* Ignore local Redis save failures */
		}
	}
}
