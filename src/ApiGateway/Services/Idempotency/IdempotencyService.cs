using ApiGateway.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ApiGateway.Services.Idempotency;

public sealed class IdempotencyService : IIdempotencyService
{
	private readonly IConnectionMultiplexer _redis;
	private readonly GatewayOptions _options;
	private readonly ILogger<IdempotencyService> _logger;

	public IdempotencyService(IConnectionMultiplexer redis, IOptions<GatewayOptions> options, ILogger<IdempotencyService> logger)
	{
		_redis = redis;
		_options = options.Value;
		_logger = logger;
	}

	public async Task<IdempotencyGuardResult> TryAcquireAsync(string? idempotencyKey, string method, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrEmpty(idempotencyKey) || (!HttpMethods.IsPost(method) && !HttpMethods.IsPut(method)))
		{
			return new IdempotencyGuardResult(IdempotencyAcquireResult.Bypassed, null, null);
		}

		try
		{
			if (!_redis.IsConnected)
			{
				return new IdempotencyGuardResult(IdempotencyAcquireResult.Bypassed, null, null);
			}

			var redisDb = _redis.GetDatabase();
			var redisIdempotencyKey = $"idempotency:{idempotencyKey}";

			var acquired = await redisDb.StringSetAsync(
				redisIdempotencyKey,
				"IN_PROGRESS",
				TimeSpan.FromSeconds(_options.IdempotencyInProgressSeconds),
				When.NotExists);

			if (acquired)
			{
				return new IdempotencyGuardResult(IdempotencyAcquireResult.Acquired, null, redisIdempotencyKey);
			}

			var existingValue = await redisDb.StringGetAsync(redisIdempotencyKey);
			if (existingValue == "IN_PROGRESS")
			{
				return new IdempotencyGuardResult(IdempotencyAcquireResult.InProgress, null, redisIdempotencyKey);
			}

			if (existingValue.HasValue)
			{
				return new IdempotencyGuardResult(
					IdempotencyAcquireResult.Replay,
					existingValue.ToString(),
					redisIdempotencyKey);
			}

			return new IdempotencyGuardResult(IdempotencyAcquireResult.Acquired, null, redisIdempotencyKey);
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Redis server unreachable for Idempotency check. Bypassing guard.");
			return new IdempotencyGuardResult(IdempotencyAcquireResult.Bypassed, null, null);
		}
	}

	public async Task TryStoreAsync(string? redisKey, string responseBody, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrEmpty(redisKey))
		{
			return;
		}

		try
		{
			if (!_redis.IsConnected)
			{
				return;
			}

			var redisDb = _redis.GetDatabase();
			await redisDb.StringSetAsync(
				redisKey,
				responseBody,
				TimeSpan.FromHours(_options.IdempotencyResultHours));
		}
		catch
		{
			/* Ignore local Redis save failures */
		}
	}
}
