namespace ApiGateway.Services.Idempotency;

public enum IdempotencyAcquireResult
{
	Bypassed,
	Acquired,
	InProgress,
	Replay
}

public sealed record IdempotencyGuardResult(
	IdempotencyAcquireResult Outcome,
	string? ReplayBody,
	string? RedisKey);

public interface IIdempotencyService
{
	Task<IdempotencyGuardResult> TryAcquireAsync(string? idempotencyKey, string method, CancellationToken cancellationToken = default);
	Task TryStoreAsync(string? redisKey, string responseBody, CancellationToken cancellationToken = default);
}
