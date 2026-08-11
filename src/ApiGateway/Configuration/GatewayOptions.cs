namespace ApiGateway.Configuration;

public sealed class GatewayOptions
{
	public const string SectionName = "Gateway";

	public int CacheAbsoluteExpirationMinutes { get; set; } = 2;
	public int IdempotencyInProgressSeconds { get; set; } = 60;
	public int IdempotencyResultHours { get; set; } = 24;

	/// <summary>
	/// Path prefixes handled by the gateway proxy policies (cache/idempotency).
	/// Keep aligned with ReverseProxy route match prefixes in appsettings.
	/// </summary>
	public List<string> ProxiedPathPrefixes { get; set; } =
	[
		"/api/v1/orders",
		"/api/v1/notifications"
	];

	public bool IsProxiedPath(string? path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return false;
		}

		return ProxiedPathPrefixes.Any(prefix =>
			path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
	}
}
