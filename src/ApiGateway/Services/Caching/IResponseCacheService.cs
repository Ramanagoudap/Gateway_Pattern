namespace ApiGateway.Services.Caching;

public interface IResponseCacheService
{
	string BuildCacheKey(string method, string? path, QueryString queryString);
	Task<string?> TryGetAsync(string cacheKey, CancellationToken cancellationToken = default);
	Task TrySetAsync(string cacheKey, string responseBody, CancellationToken cancellationToken = default);
}
