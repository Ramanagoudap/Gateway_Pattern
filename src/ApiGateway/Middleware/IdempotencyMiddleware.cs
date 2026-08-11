using System.Net;
using ApiGateway.Configuration;
using ApiGateway.Services.Idempotency;
using Microsoft.Extensions.Options;

namespace ApiGateway.Middleware;

/// <summary>
/// Redis Idempotency-Key guard for proxied POST/PUT. Replays stored responses or returns 409 while in progress.
/// </summary>
public sealed class IdempotencyMiddleware
{
	private readonly RequestDelegate _next;

	public IdempotencyMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context, IIdempotencyService idempotencyService, IOptions<GatewayOptions> options)
	{
		var path = context.Request.Path.Value;
		if (!options.Value.IsProxiedPath(path))
		{
			await _next(context);
			return;
		}

		var method = context.Request.Method;
		var idempotencyKey = context.Request.Headers["Idempotency-Key"].FirstOrDefault();
		var guard = await idempotencyService.TryAcquireAsync(idempotencyKey, method, context.RequestAborted);

		if (guard.Outcome == IdempotencyAcquireResult.InProgress)
		{
			context.Response.StatusCode = (int)HttpStatusCode.Conflict;
			await context.Response.WriteAsJsonAsync(new { error = "Concurrent request in progress." });
			return;
		}

		if (guard.Outcome == IdempotencyAcquireResult.Replay && guard.ReplayBody is not null)
		{
			context.Response.Headers["X-Cache"] = "IDEMPOTENT_HIT";
			context.Response.ContentType = "application/json";
			await context.Response.WriteAsync(guard.ReplayBody);
			return;
		}

		if (guard.Outcome != IdempotencyAcquireResult.Acquired)
		{
			await _next(context);
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

			if (context.Response.StatusCode is >= 200 and < 300)
			{
				await idempotencyService.TryStoreAsync(guard.RedisKey, responseBody, context.RequestAborted);
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

public static class IdempotencyMiddlewareExtensions
{
	public static IApplicationBuilder UseIdempotencyPolicy(this IApplicationBuilder app) =>
		app.UseMiddleware<IdempotencyMiddleware>();
}
