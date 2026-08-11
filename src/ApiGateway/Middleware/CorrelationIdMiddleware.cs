namespace ApiGateway.Middleware;

public sealed class CorrelationIdMiddleware
{
	public const string HeaderName = "X-Correlation-ID";
	public const string ItemKey = "CorrelationId";

	private readonly RequestDelegate _next;
	private readonly ILogger<CorrelationIdMiddleware> _logger;

	public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var correlationId = context.Request.Headers[HeaderName].FirstOrDefault() ?? Guid.NewGuid().ToString();
		context.Items[ItemKey] = correlationId;
		context.Response.Headers[HeaderName] = correlationId;

		using (_logger.BeginScope(new Dictionary<string, object> { [ItemKey] = correlationId }))
		{
			_logger.LogInformation("HTTP {Method} {Path} initiated", context.Request.Method, context.Request.Path);

			await _next(context);

			_logger.LogInformation("HTTP {Method} {Path} completed with status {StatusCode}",
				context.Request.Method, context.Request.Path, context.Response.StatusCode);
		}
	}
}

public static class CorrelationIdMiddlewareExtensions
{
	public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app) =>
		app.UseMiddleware<CorrelationIdMiddleware>();
}
