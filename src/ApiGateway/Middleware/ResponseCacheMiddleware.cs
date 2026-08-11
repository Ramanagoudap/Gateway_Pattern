using ApiGateway.Configuration;
using ApiGateway.Services.Caching;
using Microsoft.Extensions.Options;

namespace ApiGateway.Middleware;

/// <summary>
/// Redis GET response cache for proxied routes. Short-circuits on HIT; stores successful GET responses after YARP.
/// </summary>
public sealed class ResponseCacheMiddleware
{
	private readonly RequestDelegate _next;

	public ResponseCacheMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context, IResponseCacheService cacheService, IOptions<GatewayOptions> options)
	{
		var path = context.Request.Path.Value;
		if (!options.Value.IsProxiedPath(path) || !HttpMethods.IsGet(context.Request.Method))
		{
			await _next(context);
			return;
		}

		var cacheKey = cacheService.BuildCacheKey(context.Request.Method, path, context.Request.QueryString);
		var cachedData = await cacheService.TryGetAsync(cacheKey, context.RequestAborted);
		if (!string.IsNullOrEmpty(cachedData))
		{
			context.Response.Headers["X-Cache"] = "HIT";
			context.Response.ContentType = "application/json";
			await context.Response.WriteAsync(cachedData);
			return;
		}

		var originalBody = context.Response.Body;
		await using var buffer = new MemoryStream();
		context.Response.Body = buffer;

		try
		{
			await _next(context);

			buffer.Position = 0;
			using var reader = new StreamReader(buffer, leaveOpen: true);
			var responseBody = await reader.ReadToEndAsync(context.RequestAborted);

			if (context.Response.StatusCode is >= 200 and < 300 && !string.IsNullOrEmpty(responseBody))
			{
				await cacheService.TrySetAsync(cacheKey, responseBody, context.RequestAborted);
			}

			buffer.Position = 0;
			context.Response.ContentLength = buffer.Length;
			await buffer.CopyToAsync(originalBody, context.RequestAborted);
		}
		finally
		{
			context.Response.Body = originalBody;
		}
	}
}

public static class ResponseCacheMiddlewareExtensions
{
	public static IApplicationBuilder UseResponseCachePolicy(this IApplicationBuilder app) =>
		app.UseMiddleware<ResponseCacheMiddleware>();
}
